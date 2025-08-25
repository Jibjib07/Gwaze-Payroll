Imports System.Data.OleDb
Imports System.Windows.Controls

Module SystemModule
    Dim conString As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\MSI 10\Downloads\Fresh (3)\Fresh\Fresh\Prototype 3.accdb"
    Public con As New OleDbConnection(conString)
End Module
