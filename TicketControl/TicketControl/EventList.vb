'Copyright © Vincent Bengtsson 2017
'All rights reserved.
'Website: https://www.mydoomsite.com/

<Serializable()> _
Public Class EventList(Of T)
    Inherits List(Of T)

    <NonSerialized()> _
    Public Event Added As EventHandler(Of CollectionChangedEventArgs(Of T))

    <NonSerialized()> _
    Public Event Cleared As EventHandler

    <NonSerialized()> _
    Public Event Removed As EventHandler(Of CollectionChangedEventArgs(Of T))

    Public Shadows Sub Add(ByVal Item As T)
        MyBase.Add(Item)
        RaiseEvent Added(Me, New CollectionChangedEventArgs(Of T)(Item))
    End Sub

    Public Shadows Sub AddRange(ByVal Items As IEnumerable(Of T))
        For Each Item As T In Items
            Me.Add(Item)
        Next
    End Sub

    Public Shadows Sub Clear()
        MyBase.Clear()
        RaiseEvent Cleared(Me, EventArgs.Empty)
    End Sub

    Public Shadows Sub Insert(ByVal Index As Integer, ByVal Item As T)
        MyBase.Insert(Index, Item)
        RaiseEvent Added(Me, New CollectionChangedEventArgs(Of T)(Item))
    End Sub

    Public Shadows Sub InsertRange(ByVal Index As Integer, ByVal Items As IEnumerable(Of T))
        For Each Item As T In Items
            Me.Insert(Index, Item)
        Next
    End Sub

    Public Shadows Function Remove(ByVal Item As T) As Boolean
        Dim Result As Boolean = MyBase.Remove(Item)
        RaiseEvent Removed(Me, New CollectionChangedEventArgs(Of T)(Item))
        Return Result
    End Function

    Public Shadows Sub RemoveAt(ByVal Index As Integer)
        Dim Item As T = Me(Index)
        MyBase.RemoveAt(Index)
        RaiseEvent Removed(Me, New CollectionChangedEventArgs(Of T)(Item))
    End Sub

    Public Shadows Sub RemoveRange(ByVal Index As Integer, ByVal Count As Integer)
        If Index < 0 OrElse Index >= Me.Count Then Throw New ArgumentOutOfRangeException("Index")
        If Count <= 0 OrElse Count > Me.Count Then Throw New ArgumentOutOfRangeException("Count")
        If Index + Count - 1 >= Me.Count Then Throw New ArgumentOutOfRangeException()

        For i = Index To Count - 1
            Me.RemoveAt(i)
        Next
    End Sub

#Region "EventArgs"
    Public Class CollectionChangedEventArgs(Of TItem)
        Inherits EventArgs

        Private _item As TItem

        Public ReadOnly Property Item As TItem
            Get
                Return _item
            End Get
        End Property

        Public Sub New(ByVal Item As TItem)
            _item = Item
        End Sub
    End Class
#End Region

End Class
