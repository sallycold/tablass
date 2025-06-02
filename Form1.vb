Imports System.Data.OleDb
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Public Class Form1

    Dim bbdd1 As OleDbConnection
    Dim sConnString As OleDbConnection
    Dim cm As OleDbCommand
    Dim da As OleDbDataAdapter
    Dim ds As DataSet
    Dim dv As New DataView
    Dim consulta As String
    Dim CN As OleDbConnection = New OleDbConnection("provider=Microsoft.ace.oledb.12.0;data source=cajas.accdb;")
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBox1.DataSource = ObtenerTablas("*")
        DataGridView1.Visible = False

    End Sub
    Friend Function ObtenerTablas(ByVal nombreInicio As String) As String()
        Dim dt As DataTable = Nothing
        Using CN
            CN.Open()
            dt = CN.GetSchema("TABLES")
        End Using
        'Para SQL Server el valor de TABLE_TYPE es BASE TABLE.
        Return (From row As DataRow In dt.Rows.Cast(Of DataRow)()
                Where CStr(row("TABLE_TYPE")) = "TABLE" AndAlso CStr(row("TABLE_NAME")) Like nombreInicio
                Order By row("TABLE_NAME")
                Select CStr(row("TABLE_NAME"))).ToArray()
    End Function
    Private Sub cargar_bd()
        ListBox1.Items.Clear()
        CN = New OleDbConnection("provider=Microsoft.ace.oledb.12.0;data source = cajas.accdb;")
        cm = New OleDbCommand("Select * FROM " & ComboBox1.Text & "", CN) ' muestra los campos de una BD en un combo
        da = New OleDbDataAdapter(cm)

        CN.Open()
        ds = New DataSet()
        da.Fill(ds)

        Dim i As Integer
        For i = 0 To ds.Tables(0).Columns.Count - 1
            ListBox1.Items.Add(ds.Tables(0).Columns(i).ColumnName) ' campos de la tabla que seleccione
        Next
        DataGridView1.DataSource = ds.Tables(0)
        CN.Close()

    End Sub



    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click

        DataGridView1.Visible = True

        ' Validación: asegurarse de que se ha ingresado una palabra clave
        If TextBox1.Text.Trim() = "" Then
            MsgBox("Debe ingresar un valor a buscar", MsgBoxStyle.Critical, "Búsqueda de un valor")
            TextBox1.Focus()
            Exit Sub
        End If

        ' Conexión a la base de datos
        Dim CN As New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=cajas.accdb;")
        Dim tabla As String = ComboBox1.Text.Trim()

        ' Construcción de los campos seleccionados
        Dim camposSQL As String = ""
        For i As Integer = 0 To ListBox2.Items.Count - 1
            camposSQL &= "[" & ListBox2.Items(i).ToString() & "],"
        Next
        ' Otar forma de hacerlo 
        ' For i As Integer = 0 To ListBox2.Items.Count - 1
         'camposSQL &= "[" & ListBox2.Items(i).ToString() & "]"
          'If i < ListBox2.Items.Count - 1 Then
           '  camposSQL &= ","
          ' End If
          'Next
        
        camposSQL = camposSQL.TrimEnd(","c) ' Elimina la última coma

        ' Construcción del patrón LIKE según el tipo de búsqueda
        Dim tipoBusqueda As String = ComboBox2.Text
        Dim palabraClave As String = TextBox1.Text.Trim()
        Dim operadorLike As String = ""

        Select Case tipoBusqueda
            Case "que comience"
                operadorLike = palabraClave & "%"
            Case "que termine"
                operadorLike = "%" & palabraClave
            Case "que contenga"
                operadorLike = "%" & palabraClave & "%"
            Case Else
                operadorLike = "%" & palabraClave & "%"
        End Select

        ' Crear cláusula WHERE con varios LIKE unidos por OR
        Dim whereClause As String = ""
        For i As Integer = 0 To ListBox2.Items.Count - 1
            whereClause &= "[" & ListBox2.Items(i).ToString() & "] LIKE ?"
            If i < ListBox2.Items.Count - 1 Then
                whereClause &= " OR "
            End If
        Next

        ' Armar la consulta final
        Dim consulta As String = "SELECT " & camposSQL & " FROM [" & tabla & "] WHERE " & whereClause

        ' Crear comando y agregar parámetros
        Dim comando As New OleDbCommand(consulta, CN)
        For i As Integer = 0 To ListBox2.Items.Count - 1
            comando.Parameters.AddWithValue("?", operadorLike)
        Next

        ' Ejecutar la consulta
        Try
            Dim adaptador As New OleDbDataAdapter(comando)
            Dim resultado As New DataTable()

            CN.Open()
            adaptador.Fill(resultado)
            DataGridView1.DataSource = resultado

            If resultado.Rows.Count = 0 Then
                MsgBox("No se encontraron registros.", MsgBoxStyle.Information, "Resultado")
            End If
        Catch ex As Exception
            MsgBox("Error: " & ex.Message, MsgBoxStyle.Critical, "Error de búsqueda")
        Finally
            CN.Close()
        End Try



    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ListBox1.SelectedItem <> "" Then
            ListBox2.Items.Add(ListBox1.SelectedItem)
            ListBox1.Items.Remove(ListBox1.SelectedItem)
            ListBox1.ClearSelected()
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If ListBox2.SelectedItem <> "" Then
            ListBox1.Items.Add(ListBox2.SelectedItem)
            ListBox2.Items.Remove(ListBox2.SelectedItem)
            ListBox2.ClearSelected()
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs)
        ' selecciona todo lo que hay en el lisbox y lo mete en el otro hay que revisar 
           Dim items As String = ""
        For i As Integer = 0 To ListBox1.Items.Count - 1
            item =  ListBox1.Items(i).ToString() 
            ListBox2.Items.Add(item)
            ListBox1.items(i).Remove()

        Next
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        cargar_bd()
        ComboBox1.Text = "Seleccione el campo a buscar"
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click    
Dim items As String = ""
For i As Integer = 0 To ListBox2.Items.Count - 1
    Dim item As String = ListBox2.Items(i).ToString()
    items &= item & ", " ' Concatenar con una coma
    ListBox1.Items.Add(item)
    ListBox2.Items.RemoveAt(i)
    i -= 1
Next
    End Sub
    Private Sub BUSQUEDA()
       
    End Sub
End Class
