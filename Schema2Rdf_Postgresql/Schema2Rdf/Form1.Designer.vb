<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.addSchemaButton = New System.Windows.Forms.Button()
        Me.ListBox1 = New System.Windows.Forms.ListBox()
        Me.remove_schemaButton = New System.Windows.Forms.Button()
        Me.title_Label = New System.Windows.Forms.Label()
        Me.Mapping_Button = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(12, 12)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(200, 20)
        Me.TextBox1.TabIndex = 0
        '
        'addSchemaButton
        '
        Me.addSchemaButton.AutoSize = True
        Me.addSchemaButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.addSchemaButton.Location = New System.Drawing.Point(218, 12)
        Me.addSchemaButton.Name = "addSchemaButton"
        Me.addSchemaButton.Size = New System.Drawing.Size(78, 23)
        Me.addSchemaButton.TabIndex = 1
        Me.addSchemaButton.Text = "Add Schema"
        Me.addSchemaButton.UseVisualStyleBackColor = True
        '
        'ListBox1
        '
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.Location = New System.Drawing.Point(12, 38)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(200, 134)
        Me.ListBox1.TabIndex = 2
        '
        'remove_schemaButton
        '
        Me.remove_schemaButton.AutoSize = True
        Me.remove_schemaButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.remove_schemaButton.Location = New System.Drawing.Point(12, 178)
        Me.remove_schemaButton.Name = "remove_schemaButton"
        Me.remove_schemaButton.Size = New System.Drawing.Size(99, 23)
        Me.remove_schemaButton.TabIndex = 3
        Me.remove_schemaButton.Text = "Remove Schema"
        Me.remove_schemaButton.UseVisualStyleBackColor = True
        '
        'title_Label
        '
        Me.title_Label.AutoSize = True
        Me.title_Label.ForeColor = System.Drawing.Color.Red
        Me.title_Label.Location = New System.Drawing.Point(12, 216)
        Me.title_Label.Name = "title_Label"
        Me.title_Label.Size = New System.Drawing.Size(47, 13)
        Me.title_Label.TabIndex = 4
        Me.title_Label.Text = "Convert:"
        '
        'Mapping_Button
        '
        Me.Mapping_Button.Location = New System.Drawing.Point(221, 247)
        Me.Mapping_Button.Name = "Mapping_Button"
        Me.Mapping_Button.Size = New System.Drawing.Size(75, 54)
        Me.Mapping_Button.TabIndex = 5
        Me.Mapping_Button.Text = "Convert"
        Me.Mapping_Button.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(307, 304)
        Me.Controls.Add(Me.Mapping_Button)
        Me.Controls.Add(Me.title_Label)
        Me.Controls.Add(Me.remove_schemaButton)
        Me.Controls.Add(Me.ListBox1)
        Me.Controls.Add(Me.addSchemaButton)
        Me.Controls.Add(Me.TextBox1)
        Me.Name = "Form1"
        Me.Text = "butto"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents addSchemaButton As System.Windows.Forms.Button
    Friend WithEvents ListBox1 As System.Windows.Forms.ListBox
    Friend WithEvents remove_schemaButton As System.Windows.Forms.Button
    Friend WithEvents title_Label As System.Windows.Forms.Label
    Friend WithEvents Mapping_Button As System.Windows.Forms.Button

End Class
