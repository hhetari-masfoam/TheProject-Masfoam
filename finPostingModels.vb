Public Class PostingRuleHeader
    Public Property PostingRuleHeaderID As Integer
End Class

Public Class PostingRuleDetail

    Public Property PostingRuleDetailID As Integer
    Public Property PostingRuleHeaderID As Integer
    Public Property LineNumber As Integer
    Public Property FixedAccountID As Integer?
    Public Property EntrySideID As Integer
    Public Property AccountSourceTypeID As Integer
    Public Property SourceAmountFieldID As Integer

    ' 🔥 هذا السطر الجديد
    Public Property IsDistributed As Boolean

End Class