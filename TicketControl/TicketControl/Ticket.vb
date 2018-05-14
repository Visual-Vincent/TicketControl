'Copyright © Vincent Bengtsson 2017
'All rights reserved.
'Website: https://www.mydoomsite.com/

Imports System.Xml.Serialization
Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.Serialization.Formatters.Binary

''' <summary>
''' A class holding information about help desk/support tickets.
''' </summary>
''' <remarks></remarks>
<Serializable()> _
Public NotInheritable Class Ticket

#Region "Fields"
    Private Shared ReadOnly TicketStatusMessages As New Dictionary(Of TicketStatus, String) From { _
        {TicketStatus.New, "New"}, _
        {TicketStatus.InProgress, "In progress"}, _
        {TicketStatus.Resolved, "Resolved"}, _
        {TicketStatus.Closed, "Closed"}, _
        {TicketStatus.Unknown, "<Error>"} _
    }

    Private _status As TicketStatus = TicketStatus.New
#End Region

#Region "Events"
    ''' <summary>
    ''' Occurs when the ticket's status has changed.
    ''' </summary>
    ''' <remarks></remarks>
    <NonSerialized()> _
    Public Event StatusChanged As EventHandler
#End Region

#Region "Properties"
    ''' <summary>
    ''' Gets or sets the date when the ticket was acknowledged by/added to the system.
    ''' </summary>
    ''' <remarks></remarks>
    Public Property AcknowledgeDate As DateTime

    ''' <summary>
    ''' Gets or sets the body of the ticket.
    ''' </summary>
    ''' <remarks></remarks>
    Public Property Body As String

    ''' <summary>
    ''' Gets or sets the ticket's unique ID.
    ''' </summary>
    ''' <remarks></remarks>
    Public Property ID As Long

    ''' <summary>
    ''' Gets or sets the sender (creator) of the ticket.
    ''' </summary>
    ''' <remarks></remarks>
    Public Property Sender As String

    ''' <summary>
    ''' Gets or sets the ticket's status.
    ''' </summary>
    ''' <remarks></remarks>
    Public Property Status As TicketStatus
        Get
            Return _status
        End Get
        Set(value As TicketStatus)
            If _status = value Then Return
            _status = value
            RaiseEvent StatusChanged(Me, EventArgs.Empty)
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the subject of the ticket.
    ''' </summary>
    ''' <remarks></remarks>
    Public Property Subject As String
#End Region

#Region "Constructors"
    ''' <summary>
    ''' Initializes a new instance of the Ticket class.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        Me.AcknowledgeDate = DateTime.Now
        Me.Body = ""
        Me.ID = 0
        Me.Sender = ""
        Me.Subject = ""
    End Sub

    ''' <summary>
    ''' Initializes a new instance of the Ticket class.
    ''' </summary>
    ''' <param name="Sender">The sender (creator) of the ticket.</param>
    ''' <param name="Subject">The subject of the ticket.</param>
    ''' <param name="Body">The body of the ticket.</param>
    ''' <param name="ID">The ticket's unique ID.</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal Sender As String, _
                   ByVal Subject As String, _
                   ByVal Body As String, _
                   ByVal ID As Long)
        Me.New()
        Me.Sender = Sender
        Me.Subject = Subject
        Me.Body = Body
        Me.ID = ID
    End Sub

    ''' <summary>
    ''' Initializes a new instance of the Ticket class.
    ''' </summary>
    ''' <param name="Sender">The sender (creator) of the ticket.</param>
    ''' <param name="Subject">The subject of the ticket.</param>
    ''' <param name="Body">The body of the ticket.</param>
    ''' <param name="ID">The ticket's unique ID.</param>
    ''' <param name="Status">The ticket's status.</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal Sender As String, _
                   ByVal Subject As String, _
                   ByVal Body As String, _
                   ByVal ID As Long, _
                   ByVal Status As TicketStatus)
        Me.New(Sender, Subject, Body, ID)
        _status = Status
    End Sub

    ''' <summary>
    ''' Initializes a new instance of the Ticket class.
    ''' </summary>
    ''' <param name="Sender">The sender (creator) of the ticket.</param>
    ''' <param name="Subject">The subject of the ticket.</param>
    ''' <param name="Body">The body of the ticket.</param>
    ''' <param name="ID">The ticket's unique ID.</param>
    ''' <param name="AcknowledgeDate">The date when the ticket was acknowledged by/added to the system.</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal Sender As String, _
                   ByVal Subject As String, _
                   ByVal Body As String, _
                   ByVal ID As Long, _
                   ByVal AcknowledgeDate As DateTime)
        Me.New(Sender, Subject, Body, ID)
        Me.AcknowledgeDate = AcknowledgeDate
    End Sub

    ''' <summary>
    ''' Initializes a new instance of the Ticket class.
    ''' </summary>
    ''' <param name="Sender">The sender (creator) of the ticket.</param>
    ''' <param name="Subject">The subject of the ticket.</param>
    ''' <param name="Body">The body of the ticket.</param>
    ''' <param name="ID">The ticket's unique ID.</param>
    ''' <param name="Status">The ticket's status.</param>
    ''' <param name="AcknowledgeDate">The date when the ticket was acknowledged by/added to the system.</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal Sender As String, _
                   ByVal Subject As String, _
                   ByVal Body As String, _
                   ByVal ID As Long, _
                   ByVal Status As TicketStatus, _
                   ByVal AcknowledgeDate As DateTime)
        Me.New(Sender, Subject, Body, ID, Status)
        Me.AcknowledgeDate = AcknowledgeDate
    End Sub
#End Region

#Region "Methods"
    ''' <summary>
    ''' Gets the ticket status as a string.
    ''' </summary>
    ''' <remarks></remarks>
    Public Function GetTicketStatus() As String
        Return Ticket.GetTicketStatus(Me.Status)
    End Function

    ''' <summary>
    ''' Gets the specified ticket status as a string.
    ''' </summary>
    ''' <param name="Status">The status which's string representation to return.</param>
    ''' <remarks></remarks>
    Public Shared Function GetTicketStatus(ByVal Status As TicketStatus) As String
        Dim StatusMessage As String = Nothing
        If Ticket.TicketStatusMessages.TryGetValue(Status, StatusMessage) = False Then
            StatusMessage = Ticket.TicketStatusMessages(TicketStatus.Unknown)
        End If
        Return StatusMessage
    End Function

    ''' <summary>
    ''' Loads a Ticket class from a file.
    ''' </summary>
    ''' <param name="FilePath">The path and file name of the file to load.</param>
    ''' <remarks></remarks>
    Public Shared Function Load(ByVal FilePath As String) As Ticket
        Using FStream As New FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read)
            Dim Serializer As New XmlSerializer(GetType(Ticket))
            Return DirectCast(Serializer.Deserialize(FStream), Ticket)
        End Using
    End Function

    ''' <summary>
    ''' Writes the class and all its data to a file in XML format.
    ''' </summary>
    ''' <param name="FilePath">The path and file name of the file to create.</param>
    ''' <remarks></remarks>
    Public Sub Save(ByVal FilePath As String)
        Using FStream As New FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None)
            Dim Serializer As New XmlSerializer(GetType(Ticket))
            Serializer.Serialize(FStream, Me)
        End Using
    End Sub

    ''' <summary>
    ''' Writes a list of tickets to a compressed binary file.
    ''' </summary>
    ''' <param name="List">The list to save.</param>
    ''' <param name="FilePath">The path and file name of the file to create.</param>
    ''' <remarks></remarks>
    Public Shared Sub SaveList(ByVal List As List(Of Ticket), ByVal FilePath As String)
        Using FStream As New FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None)
            Using GZStream As New GZipStream(FStream, CompressionMode.Compress)
                Dim Formatter As New BinaryFormatter
                Formatter.Serialize(GZStream, List)
            End Using
        End Using
    End Sub

    ''' <summary>
    ''' Loads a list of tickets from a compressed file.
    ''' </summary>
    ''' <param name="FilePath">The path and file name of the file to create.</param>
    ''' <remarks></remarks>
    Public Shared Function LoadList(ByVal FilePath As String) As List(Of Ticket)
        Using FStream As New FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read)
            Using GZStream As New GZipStream(FStream, CompressionMode.Decompress)
                Dim Formatter As New BinaryFormatter
                Return DirectCast(Formatter.Deserialize(GZStream), List(Of Ticket))
            End Using
        End Using
    End Function
#End Region

#Region "Enumerations"
    Public Enum TicketStatus As Integer
        ''' <summary>
        ''' Unknown ticket status.
        ''' </summary>
        ''' <remarks></remarks>
        Unknown = -1

        ''' <summary>
        ''' Indicates that a ticket is new.
        ''' </summary>
        ''' <remarks></remarks>
        [New] = 0

        ''' <summary>
        ''' Indicates that a ticket is currently being processed.
        ''' </summary>
        ''' <remarks></remarks>
        InProgress

        ''' <summary>
        ''' Indicates that a ticket is resolved.
        ''' </summary>
        ''' <remarks></remarks>
        Resolved

        ''' <summary>
        ''' Indicates that a ticket is closed.
        ''' </summary>
        ''' <remarks></remarks>
        Closed
    End Enum
#End Region

End Class
