Imports TicketControl

Public Class Form1

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Dim TicketCtrl As New TicketItem(New Ticket("john@example.com", _
                                                    "Problem with XYZ", _
                                                    "Lorem ipsum dolor sit amet aliquam " & _
                                                    "Amet purus in nibh penatibus vitae sit ac dui phasellus quam fusce. " & _
                                                    "Eleifend tempus similique nulla vel sed. Diam platea dictumst " & _
                                                    "iaculis et nulla. Odio irure erat. Consequat turpis purus sit " & _
                                                    "placerat et molestie quidem diam magni in tellus. " & _
                                                    "Sociosqu ligula risus commodo dolor id. " & _
                                                    "Aliquam massa leo. Nam viverra erat.", _
                                                    New Random().Next(0, Integer.MaxValue), _
                                                    New Random().Next(0, 4)))
        Me.TicketContainer1.Tickets.Add(TicketCtrl)
        TicketCtrl.BringToFront()
    End Sub
End Class
