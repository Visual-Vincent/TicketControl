'Copyright © Vincent Bengtsson 2017
'All rights reserved.
'Website: https://www.mydoomsite.com/

Imports System.Windows.Forms
Imports System.ComponentModel
Imports System.Drawing

Public Class TicketContainer
    Inherits Control

#Region "Fields"
    Private WithEvents _tickets As New EventList(Of TicketItem)
    Private TicketPanel As New Panel With {.Dock = DockStyle.Fill, .Margin = New Padding(0), .AutoScroll = True}
#End Region

#Region "Properties"
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), _
    Obsolete("Use the ""Tickets"" property instead.", True)> _
    Public Shadows Property Controls As Control.ControlCollection

    ''' <summary>
    ''' Gets the list of tickets added to this control.
    ''' </summary>
    ''' <remarks></remarks>
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> _
    Public ReadOnly Property Tickets As EventList(Of TicketItem)
        Get
            Return _tickets
        End Get
    End Property
#End Region

#Region "Constructors"
    ''' <summary>
    ''' Initializes a new instance of the TicketContainer class.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        Me.BackColor = Color.FromKnownColor(KnownColor.Window)
        MyBase.Controls.Add(Me.TicketPanel)
    End Sub
#End Region

#Region "Event handlers"
    Private Sub Tickets_Added(sender As Object, e As EventList(Of TicketItem).CollectionChangedEventArgs(Of TicketItem)) Handles _tickets.Added
        With e.Item
            .Dock = Windows.Forms.DockStyle.Top
            .InnerMarginColor = Color.FromKnownColor(KnownColor.Window)
            If .InnerMargin.Bottom < 4 Then .InnerMargin = New Padding(.Padding.Left, .Padding.Top, .Padding.Right, 4)
        End With
        Me.TicketPanel.Controls.Add(e.Item)
    End Sub

    Private Sub Tickets_Cleared(sender As Object, e As System.EventArgs) Handles _tickets.Cleared
        Me.TicketPanel.Controls.Clear()
    End Sub

    Private Sub Tickets_Removed(sender As Object, e As EventList(Of TicketItem).CollectionChangedEventArgs(Of TicketItem)) Handles _tickets.Removed
        Me.TicketPanel.Controls.Remove(e.Item)
    End Sub
#End Region

End Class
