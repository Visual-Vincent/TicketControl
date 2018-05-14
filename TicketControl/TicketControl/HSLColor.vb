'Copyright © Vincent Bengtsson 2016-2017
'All rights reserved.
'Website: https://www.mydoomsite.com/

Imports System.Drawing
Imports System.Runtime.InteropServices

Friend Class HSLColor
    <DllImport("shlwapi.dll")> _
    Private Shared Function ColorHLSToRGB(H As Integer, L As Integer, S As Integer) As Integer
    End Function

    <DllImport("shlwapi.dll")> _
    Private Shared Sub ColorRGBToHLS(RGB As Integer, ByRef H As Integer, ByRef L As Integer, ByRef S As Integer)
    End Sub

    Private _hue As Integer
    Private _saturation As Integer
    Private _luminance As Integer

    Public Property Hue As Integer
        Get
            Return _hue
        End Get
        Set(value As Integer)
            If value > 240 Then
                value = 240
            ElseIf value < 0 Then
                value = 0
            End If
            _hue = value
        End Set
    End Property

    Public Property Saturation As Integer
        Get
            Return _saturation
        End Get
        Set(value As Integer)
            If value > 240 Then
                value = 240
            ElseIf value < 0 Then
                value = 0
            End If
            _saturation = value
        End Set
    End Property

    Public Property Luminance As Integer
        Get
            Return _luminance
        End Get
        Set(value As Integer)
            If value > 240 Then
                value = 240
            ElseIf value < 0 Then
                value = 0
            End If
            _luminance = value
        End Set
    End Property

    Public Shared Function FromColor(ByVal Color As Color) As HSLColor
        Dim H, S, L As Integer
        ColorRGBToHLS(ColorTranslator.ToWin32(Color), H, L, S)
        Return New HSLColor(H, S, L)
    End Function

    Public Function ToColor() As Color
        Return ColorTranslator.FromWin32(ColorHLSToRGB(Me.Hue, Me.Luminance, Me.Saturation))
    End Function

    Public Overrides Function ToString() As String
        Return String.Format("HSL:{{{0},{1},{2}}}", Me.Hue, Me.Saturation, Me.Luminance)
    End Function

    Public Sub New()
    End Sub

    Public Sub New(ByVal Color As Color)
        Dim HSL As HSLColor = HSLColor.FromColor(Color)
        Me.Hue = HSL.Hue
        Me.Saturation = HSL.Saturation
        Me.Luminance = HSL.Luminance
    End Sub

    Public Sub New(ByVal Hue As Integer, ByVal Saturation As Integer, ByVal Luminance As Integer)
        Me.Hue = Hue
        Me.Saturation = Saturation
        Me.Luminance = Luminance
    End Sub
End Class