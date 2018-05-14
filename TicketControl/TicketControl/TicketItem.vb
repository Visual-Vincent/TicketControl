'Copyright © Vincent Bengtsson 2017
'All rights reserved.
'Website: https://www.mydoomsite.com/

Imports System.Windows.Forms
Imports System.ComponentModel
Imports System.Drawing

<DefaultProperty("MouseColorIsBasedOnStatus")> _
Public Class TicketItem
    Inherits Control

#Region "Fields"
    Private _bodyTextColor As Color = Color.Gray
    Private _innerMargin As New Padding(0)
    Private _innerMarginColor As Color = Color.FromKnownColor(KnownColor.Control)
    Private _mouseColorBasedOnStatus As Boolean = True
    Private _mouseDownColor As Color = Color.FromArgb(255, 235, 235, 235)
    Private _mouseOverColor As Color = Color.FromArgb(255, 245, 245, 245)
    Private _subjectFont As New Font(Me.Font.FontFamily, 14.0F, FontStyle.Bold)
    Private _ticket As Ticket = Nothing
    Private _ticketBackColor As Color = Color.FromKnownColor(KnownColor.Window)

    Private BodyTextBrush As New SolidBrush(_bodyTextColor)
    Private MouseState As MouseStateEnum = MouseStateEnum.None
    Private MouseDownBrush As New SolidBrush(_mouseDownColor)
    Private MouseOverBrush As New SolidBrush(_mouseOverColor)
    Private TicketBackgroundBrush As New SolidBrush(_ticketBackColor)
    Private TextBrush As New SolidBrush(Me.ForeColor)
    Private TicketBrush As New SolidBrush(TicketItem.GetTicketColor(_ticket))
    Private TicketPen As New Pen(TicketItem.GetTicketColor(_ticket))
    Private TargetRectangle As New Rectangle(Point.Empty, Me.ClientSize)

    Private Const ColorRectangleWidth As Integer = 4
    Private Const ArrowHeight As Integer = 17
    Private Const ArrowWidth As Integer = 10

    Private Shared ReadOnly BodyFormat As New StringFormat(StringFormatFlags.NoClip) With {.Trimming = StringTrimming.EllipsisCharacter}

    Public Shared ReadOnly ArrowPen As New Pen(Brushes.Black, 2.0F) With {.EndCap = Drawing2D.LineCap.Triangle}
    Public Shared ReadOnly TicketNewColor As Color = Color.FromArgb(255, 0, 127, 255)
    Public Shared ReadOnly TicketInProgressColor As Color = Color.FromArgb(255, 255, 200, 0)
    Public Shared ReadOnly TicketResolvedColor As Color = Color.FromArgb(255, 0, 225, 0)
    Public Shared ReadOnly TicketClosedColor As Color = Color.FromArgb(255, 255, 0, 0)
#End Region

#Region "Properties"
    ''' <summary>
    ''' Use the InnerMarginColor property instead.
    ''' </summary>
    ''' <remarks></remarks>
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Overrides Property BackColor As System.Drawing.Color
        Get
            Return MyBase.BackColor
        End Get
        Set(value As System.Drawing.Color)
            _innerMarginColor = value
            MyBase.BackColor = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the color of the ticket body excerpt displayed in the ticket.
    ''' </summary>
    ''' <remarks></remarks>
    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), _
    DefaultValue(GetType(Color), "Gray"), _
    Description("The color of the ticket body excerpt displayed in the ticket.")> _
    Public Property BodyTextColor As Color
        Get
            Return _bodyTextColor
        End Get
        Set(value As Color)
            _bodyTextColor = value

            If Me.BodyTextBrush IsNot Nothing Then Me.BodyTextBrush.Dispose()
            Me.BodyTextBrush = New SolidBrush(_bodyTextColor)

            Me.Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Gets the default size of the control.
    ''' </summary>
    ''' <remarks></remarks>
    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Protected Overrides ReadOnly Property DefaultSize As System.Drawing.Size
        Get
            Return New Size(320, 140)
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets the color of the text displayed in the ticket.
    ''' </summary>
    ''' <remarks></remarks>
    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), _
    DefaultValue(GetType(Color), "ControlText"), _
    Description("The color of the text displayed in the ticket.")> _
    Public Overrides Property ForeColor As System.Drawing.Color
        Get
            Return MyBase.ForeColor
        End Get
        Set(value As System.Drawing.Color)
            MyBase.ForeColor = value

            If Me.TextBrush IsNot Nothing Then Me.TextBrush.Dispose()
            Me.TextBrush = New SolidBrush(value)

            Me.Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the space between the displayed ticket and the control's edges.
    ''' </summary>
    ''' <remarks></remarks>
    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), _
    DefaultValue(GetType(Padding), "{Left=0,Top=0,Right=0,Bottom=0}"), _
    Description("The space between the displayed ticket and the control's edges.")> _
    Public Property InnerMargin As Padding
        Get
            Return _innerMargin
        End Get
        Set(value As Padding)
            _innerMargin = value
            Me.RefreshTargetRectangle()
            Me.Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the color displayed around the ticket when the InnerMargin property is set.
    ''' </summary>
    ''' <remarks></remarks>
    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), _
    DefaultValue(GetType(Color), "Control"), _
    Description("The color displayed around the ticket when the InnerMargin property is set.")> _
    Public Property InnerMarginColor As Color
        Get
            Return _innerMarginColor
        End Get
        Set(value As Color)
            Me.BackColor = value
            Me.Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets whether the MouseDown/MouseOver colors are based on the ticket's status rather than 
    ''' by the MouseDownColor/MouseOverColor properties.
    ''' </summary>
    ''' <remarks></remarks>
    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), _
    DefaultValue(True), _
    Description("Whether the MouseDown/MouseOver colors are based on the ticket's status rather than " & _
                "by the MouseDownColor/MouseOverColor properties.")> _
    Public Property MouseColorIsBasedOnStatus As Boolean
        Get
            Return _mouseColorBasedOnStatus
        End Get
        Set(value As Boolean)
            _mouseColorBasedOnStatus = value
            Me.RefreshMouseBrushes()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the background color that the control should switch to when the mouse is held down on it.
    ''' </summary>
    ''' <remarks></remarks>
    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), _
    Description("The background color that the control should switch to when the mouse is held down on it.")> _
    Public Property MouseDownColor As Color
        Get
            Return _mouseDownColor
        End Get
        Set(value As Color)
            _mouseDownColor = value

            If Me.MouseColorIsBasedOnStatus = False Then
                If Me.MouseDownBrush IsNot Nothing Then Me.MouseDownBrush.Dispose()
                Me.MouseDownBrush = New SolidBrush(_mouseDownColor)

                If Me.MouseState = MouseStateEnum.Down Then Me.Invalidate()
            End If
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the background color that the control should switch to when the mouse is inside the control's bounds.
    ''' </summary>
    ''' <remarks></remarks>
    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), _
    Description("The background color that the control should switch to when the mouse is inside the control's bounds.")> _
    Public Property MouseOverColor As Color
        Get
            Return _mouseOverColor
        End Get
        Set(value As Color)
            _mouseOverColor = value

            If Me.MouseColorIsBasedOnStatus = False Then
                If Me.MouseOverBrush IsNot Nothing Then Me.MouseOverBrush.Dispose()
                Me.MouseOverBrush = New SolidBrush(_mouseOverColor)

                If Me.MouseState = MouseStateEnum.Over Then Me.Invalidate()
            End If
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the font of the Subject-part of the ticket.
    ''' </summary>
    ''' <remarks></remarks>
    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), _
    Description("The font of the Subject-part of the ticket.")> _
    Public Property SubjectFont As Font
        Get
            Return _subjectFont
        End Get
        Set(value As Font)
            If value Is _subjectFont Then Return
            If _subjectFont IsNot Nothing Then _subjectFont.Dispose()

            _subjectFont = value
            Me.Invalidate()
        End Set
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), _
    Obsolete("This property is not implemented.", True)> _
    Public Overrides Property Text As String
        Get
            Return ""
        End Get
        Set(value As String)
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the ticket which's information is displayed by the control.
    ''' </summary>
    ''' <remarks></remarks>
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Property Ticket As Ticket
        Get
            Return _ticket
        End Get
        Set(value As Ticket)
            If _ticket IsNot Nothing Then RemoveHandler _ticket.StatusChanged, AddressOf Ticket_StatusChanged
            _ticket = value

            If _ticket IsNot Nothing Then
                AddHandler _ticket.StatusChanged, AddressOf Ticket_StatusChanged
            End If

            If Me.TicketBrush IsNot Nothing Then Me.TicketBrush.Dispose()
            If Me.TicketPen IsNot Nothing Then Me.TicketPen.Dispose()
            Me.TicketBrush = New SolidBrush(TicketItem.GetTicketColor(Me.Ticket))
            Me.TicketPen = New Pen(TicketItem.GetTicketColor(Me.Ticket))

            Me.RefreshMouseBrushes(False)

            Me.Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the background color of the displayed ticket.
    ''' </summary>
    ''' <remarks></remarks>
    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), _
    DefaultValue(GetType(Color), "Window"), _
    Description("The background color of the displayed ticket.")> _
    Public Property TicketBackColor As Color
        Get
            Return _ticketBackColor
        End Get
        Set(value As Color)
            _ticketBackColor = value

            If Me.TicketBackgroundBrush IsNot Nothing Then Me.TicketBackgroundBrush.Dispose()
            Me.TicketBackgroundBrush = New SolidBrush(_ticketBackColor)

            Me.Invalidate()
        End Set
    End Property
#End Region

#Region "Constructors"
    ''' <summary>
    ''' Initializes a new instance of the TicketItem class.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True) 'To reduce flickering.
        Me.Cursor = Cursors.Hand
    End Sub

    ''' <summary>
    ''' Initializes a new instance of the TicketItem class.
    ''' </summary>
    ''' <param name="Ticket">The ticket which's information is displayed by the control.</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal Ticket As Ticket)
        Me.New()
        Me.Ticket = Ticket
    End Sub
#End Region


#Region "Overridden methods"

#Region "Mouse events"
    Protected Overrides Sub OnMouseDown(e As System.Windows.Forms.MouseEventArgs)
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Me.MouseState = MouseStateEnum.Down
            Me.Invalidate()
        End If
        MyBase.OnMouseDown(e)
    End Sub

    Protected Overrides Sub OnMouseEnter(e As System.EventArgs)
        Me.MouseState = If(Control.MouseButtons = Windows.Forms.MouseButtons.Left, MouseStateEnum.Down, MouseStateEnum.Over)
        Me.Invalidate()
        MyBase.OnMouseEnter(e)
    End Sub

    Protected Overrides Sub OnMouseLeave(e As System.EventArgs)
        Me.MouseState = MouseStateEnum.None
        Me.Invalidate()
        MyBase.OnMouseLeave(e)
    End Sub

    Protected Overrides Sub OnMouseUp(e As System.Windows.Forms.MouseEventArgs)
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Me.MouseState = MouseStateEnum.Over
            Me.Invalidate()
        End If
        MyBase.OnMouseUp(e)
    End Sub
#End Region

#Region "Painting"
    Protected Overrides Sub OnPaint(e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)

        Dim DrawPoint As Point = Me.TargetRectangle.Location
        Dim DrawSize As Size = Me.TargetRectangle.Size

        Dim LeftInnerCoordinate As Integer = DrawPoint.X + TicketItem.ColorRectangleWidth
        Dim RightCoordinate As Integer = Me.TargetRectangle.Right
        Dim BottomCoordinate As Integer = Me.TargetRectangle.Bottom

        e.Graphics.DrawRectangle(Me.TicketPen, New Rectangle(DrawPoint, New Size(DrawSize.Width - 1, DrawSize.Height - 1)))
        e.Graphics.FillRectangle(Me.TicketBrush, New Rectangle(DrawPoint, New Size(TicketItem.ColorRectangleWidth, DrawSize.Height - 1)))

        If Me.Ticket Is Nothing Then
            Dim UnknownTicket As String = "(unknown ticket)"
            Dim UnknownTicketSize As SizeF = e.Graphics.MeasureString(UnknownTicket, Me.SubjectFont)

            e.Graphics.DrawString(UnknownTicket, Me.SubjectFont, Brushes.Gray, _
                                  New PointF(DrawPoint.X + 16.0F, DrawPoint.Y + (DrawSize.Height / 2.0F) - (UnknownTicketSize.Height / 2.0F)))
            Return
        End If

        'Information about the data displayed on the ticket.
        Dim TicketColor As Color = TicketItem.GetTicketColor(Me.Ticket)
        Dim TicketID As String = "Ticket ID: #" & Me.Ticket.ID

        Dim AcknowledgeDate As String = "Acknowledged: " & Me.Ticket.AcknowledgeDate.ToString("yyyy-MM-dd  HH:mm:ss")
        Dim AcknowledgeDateSize As SizeF = e.Graphics.MeasureString(AcknowledgeDate, Me.Font)
        Dim AchnowledgeDateLocation As New PointF(RightCoordinate - 4.0F - AcknowledgeDateSize.Width, DrawPoint.Y + 4.0F)

        Dim Sender As String = "Created by: " & Me.Ticket.Sender
        Dim SenderSize As SizeF = e.Graphics.MeasureString(Sender, Me.Font)

        Dim Subject As String = Me.Ticket.Subject
        Dim SubjectSize As SizeF = e.Graphics.MeasureString(Subject, Me.SubjectFont)
        Dim SubjectLocation As New PointF(LeftInnerCoordinate + 16.0F, DrawPoint.Y + 4.0F + AcknowledgeDateSize.Height + 6.0F)

        Dim Body As String = Me.Ticket.Body
        Dim BodyBounds As New RectangleF(New PointF(SubjectLocation.X, SubjectLocation.Y + SubjectSize.Height + 2.0F), _
                                         New SizeF((RightCoordinate - ArrowWidth - 16.0F) - SubjectLocation.X, AcknowledgeDateSize.Height * 2.0F))

        Dim Status As String = "Status: " & Me.Ticket.GetTicketStatus()
        Dim StatusSize As SizeF = e.Graphics.MeasureString(Status, Me.Font)

        'Information about how the '>' arrow should be drawn.
        Dim ArrowBounds As New Rectangle(New Point(RightCoordinate - ArrowWidth - 10, _
                                                   DrawPoint.Y + (DrawSize.Height \ 2) - (ArrowHeight \ 2)), _
                                         New Size(ArrowWidth, ArrowHeight))

        Dim ArrowPoint1 As Point = ArrowBounds.Location
        Dim ArrowPoint2 As New Point(ArrowBounds.Right, ArrowBounds.Location.Y + (ArrowBounds.Height \ 2))
        Dim ArrowPoint3 As New Point(ArrowBounds.Location.X, ArrowBounds.Bottom)

        'Top - Ticket ID and Acknowledge date.
        e.Graphics.DrawString(TicketID, Me.Font, Me.TextBrush, New PointF(LeftInnerCoordinate + 4.0F, DrawPoint.Y + 4.0F))
        e.Graphics.DrawString(AcknowledgeDate, Me.Font, Me.TextBrush, AchnowledgeDateLocation)

        'Bottom - Ticket status and Sender.
        e.Graphics.DrawString(Status, Me.Font, Me.TicketBrush, New PointF(LeftInnerCoordinate + 4.0F, BottomCoordinate - StatusSize.Height - 4.0F))
        e.Graphics.DrawString(Sender, Me.Font, Me.TextBrush, New PointF(RightCoordinate - 4.0F - SenderSize.Width, BottomCoordinate - SenderSize.Height - 4.0F))

        'Middle - Subject and Body.
        e.Graphics.DrawString(Subject, Me.SubjectFont, Me.TextBrush, SubjectLocation)
        e.Graphics.DrawString(Body, Me.Font, Me.BodyTextBrush, BodyBounds, TicketItem.BodyFormat)

        'The '>' arrow.
        e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        e.Graphics.DrawLine(TicketItem.ArrowPen, ArrowPoint1, ArrowPoint2)
        e.Graphics.DrawLine(TicketItem.ArrowPen, ArrowPoint2, ArrowPoint3)
    End Sub

    Protected Overrides Sub OnPaintBackground(e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaintBackground(e)

        Dim BackgroundBrush As Brush = Nothing
        Select Case Me.MouseState
            Case MouseStateEnum.Down : BackgroundBrush = Me.MouseDownBrush
            Case MouseStateEnum.Over : BackgroundBrush = Me.MouseOverBrush
            Case Else : BackgroundBrush = Me.TicketBackgroundBrush
        End Select

        e.Graphics.FillRectangle(BackgroundBrush, Me.TargetRectangle)
    End Sub
#End Region

#Region "Size and Position"
    Protected Overrides Sub OnSizeChanged(e As System.EventArgs)
        Me.RefreshTargetRectangle()
        MyBase.OnSizeChanged(e)
    End Sub
#End Region

#End Region

#Region "Methods"
    ''' <summary>
    ''' Gets the color related to the specified ticket (determined by the ticket's status).
    ''' </summary>
    ''' <param name="Ticket">The ticket which's color to determine.</param>
    ''' <remarks></remarks>
    Public Shared Function GetTicketColor(ByVal Ticket As Ticket) As Color
        If Ticket IsNot Nothing Then
            Return TicketItem.GetTicketColor(Ticket.Status)
        End If
        Return TicketItem.GetTicketColor(Ticket.TicketStatus.Unknown)
    End Function

    ''' <summary>
    ''' Gets the color related to the specified ticket status.
    ''' </summary>
    ''' <param name="Status">The status which's color to determine.</param>
    ''' <remarks></remarks>
    Public Shared Function GetTicketColor(ByVal Status As Ticket.TicketStatus) As Color
        Select Case Status
            Case TicketControl.Ticket.TicketStatus.New : Return TicketItem.TicketNewColor
            Case TicketControl.Ticket.TicketStatus.InProgress : Return TicketItem.TicketInProgressColor
            Case TicketControl.Ticket.TicketStatus.Resolved : Return TicketItem.TicketResolvedColor
            Case TicketControl.Ticket.TicketStatus.Closed : Return TicketItem.TicketClosedColor
        End Select
        Return Color.Gray 'Failure, which is highly unlikely.
    End Function

    ''' <summary>
    ''' Gets the color for the specified ticket for when the mouse is held down on it.
    ''' </summary>
    ''' <param name="Ticket">The ticket which's MouseDown color to get.</param>
    ''' <remarks></remarks>
    Protected Shared Function GetTicketMouseDownColor(ByVal Ticket As Ticket) As Color 'Darker
        Const WantedLuminance As Integer = 221 '~92%
        Const LuminanceThreshold As Integer = 204 '85%

        Dim HSL As New HSLColor(TicketItem.GetTicketColor(Ticket))
        If HSL.Luminance >= LuminanceThreshold Then
            HSL.Luminance = 161 '~67%
        Else
            HSL.Luminance = WantedLuminance
        End If

        Return HSL.ToColor()
    End Function

    ''' <summary>
    ''' Gets the color for the specified ticket for when the mouse is located inside its bounds.
    ''' </summary>
    ''' <param name="Ticket">The ticket which's MouseOver color to get.</param>
    ''' <remarks></remarks>
    Protected Shared Function GetTicketMouseOverColor(ByVal Ticket As Ticket) As Color 'Lighter
        Const WantedLuminance As Integer = 228 '95%
        Const LuminanceThreshold As Integer = 204 '85%

        Dim HSL As New HSLColor(TicketItem.GetTicketColor(Ticket))
        If HSL.Luminance >= LuminanceThreshold Then
            HSL.Luminance = 180 '75%
        Else
            HSL.Luminance = WantedLuminance
        End If

        Return HSL.ToColor()
    End Function

    ''' <summary>
    ''' Re-determines the colors of the brushes used to paint the MouseDown/MouseOver effects on the control.
    ''' </summary>
    ''' <param name="AllowRepaint">Optional. Whether or not to allow the control to be repainted (if necessary).</param>
    ''' <remarks></remarks>
    Private Sub RefreshMouseBrushes(Optional ByVal AllowRepaint As Boolean = True)
        If Me.MouseDownBrush IsNot Nothing Then Me.MouseDownBrush.Dispose()
        If Me.MouseOverBrush IsNot Nothing Then Me.MouseOverBrush.Dispose()

        If Me.MouseColorIsBasedOnStatus = True Then
            Me.MouseDownBrush = New SolidBrush(TicketItem.GetTicketMouseDownColor(Me.Ticket))
            Me.MouseOverBrush = New SolidBrush(TicketItem.GetTicketMouseOverColor(Me.Ticket))
        Else
            Me.MouseDownBrush = New SolidBrush(_mouseDownColor)
            Me.MouseOverBrush = New SolidBrush(_mouseOverColor)
        End If

        If AllowRepaint = True AndAlso _
            Me.MouseState = MouseStateEnum.Down OrElse _
             Me.MouseState = MouseStateEnum.Over Then Me.Invalidate()
    End Sub

    ''' <summary>
    ''' Recalculates the bounds of the rectangle in which to render the control.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub RefreshTargetRectangle()
        Me.TargetRectangle = New Rectangle(_innerMargin.Left, _innerMargin.Top, _
                                           Me.ClientSize.Width - _innerMargin.Left - _innerMargin.Right, _
                                           Me.ClientSize.Height - _innerMargin.Top - _innerMargin.Bottom)
    End Sub
#End Region

#Region "Event handlers"

#Region "Ticket"
    Private Sub Ticket_StatusChanged(sender As Object, e As System.EventArgs)
        If Me.TicketBrush IsNot Nothing Then Me.TicketBrush.Dispose()
        If Me.TicketPen IsNot Nothing Then Me.TicketPen.Dispose()

        Me.TicketBrush = New SolidBrush(TicketItem.GetTicketColor(Me.Ticket))
        Me.TicketPen = New Pen(TicketItem.GetTicketColor(Me.Ticket))

        Me.RefreshMouseBrushes(False)
        Me.Invalidate()
    End Sub
#End Region

#End Region


#Region "Enumerations"
    Private Enum MouseStateEnum As Integer
        ''' <summary>
        ''' No mouse state.
        ''' </summary>
        ''' <remarks></remarks>
        None = 0

        ''' <summary>
        ''' Indicates that the mouse is currently over the control (inside the control's bounds).
        ''' </summary>
        ''' <remarks></remarks>
        Over

        ''' <summary>
        ''' Indicates that the mouse is being held down on the control.
        ''' </summary>
        ''' <remarks></remarks>
        Down
    End Enum
#End Region

End Class
