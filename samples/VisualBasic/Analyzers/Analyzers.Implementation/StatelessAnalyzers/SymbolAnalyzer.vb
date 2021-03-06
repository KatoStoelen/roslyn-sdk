﻿' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.Collections.Immutable
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Diagnostics

Namespace BasicAnalyzers
    ''' <summary>
    ''' Analyzer for reporting symbol diagnostics.
    ''' It reports diagnostics for named type symbols that have members with the same name as the named type.
    ''' </summary>
    ''' <remarks>
    ''' For analyzers that requires analyzing symbols or syntax nodes across compilation, see <see cref="CompilationStartedAnalyzer"/> and <see cref="CompilationStartedAnalyzerWithCompilationWideAnalysis"/>.
    ''' </remarks>
    <DiagnosticAnalyzer(LanguageNames.VisualBasic)>
    Public Class SymbolAnalyzer
        Inherits DiagnosticAnalyzer

#Region "Descriptor fields"
        Friend Shared ReadOnly Title As LocalizableString = "Do not declare members with same name as containing type"
        Friend Shared ReadOnly MessageFormat As LocalizableString = "Type '{0}' has one or more members with the same name, considering renaming the type or the members."
        Friend Shared ReadOnly Description As LocalizableString = "Do not declare members with same name as containing type."

        Friend Shared Rule As New DiagnosticDescriptor(DiagnosticIds.SymbolAnalyzerRuleId, Title, MessageFormat, DiagnosticCategories.Stateless, DiagnosticSeverity.Warning, isEnabledByDefault:=True, description:=Description)
#End Region

        Public Overrides ReadOnly Property SupportedDiagnostics() As ImmutableArray(Of DiagnosticDescriptor)
            Get
                Return ImmutableArray.Create(Rule)
            End Get
        End Property

        Public Overrides Sub Initialize(context As AnalysisContext)
            context.RegisterSymbolAction(AddressOf AnalyzeSymbol, SymbolKind.NamedType)
        End Sub

        Private Shared Sub AnalyzeSymbol(context As SymbolAnalysisContext)
            Dim namedTypeSymbol = DirectCast(context.Symbol, INamedTypeSymbol)

            ' Find just those named type symbols that have members with the same name as the named type.
            If namedTypeSymbol.GetMembers(namedTypeSymbol.Name).Any() Then
                ' For all such symbols, report a diagnostic.
                Dim diag = Diagnostic.Create(Rule, namedTypeSymbol.Locations(0), namedTypeSymbol.Name)
                context.ReportDiagnostic(diag)
            End If
        End Sub
    End Class
End Namespace
