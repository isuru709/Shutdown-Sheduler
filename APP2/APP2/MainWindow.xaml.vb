Imports Microsoft.Win32
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Threading.Tasks
Imports System.Windows.Threading
Imports System.Diagnostics
Namespace ShutdownScheduler


    Partial Class MainWindow

        Inherits Window
        Private sleepProcess As Process = Nothing
        Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            MinuteBox.ItemsSource = New String() {
            "5 min",
            "10 min",
            "15 min",
            "30 min",
            "45 min",
            "1 hour",
            "1 hour 30 min",
            "2 hour",
            "2 hour 30 min",
            "3 hour",
            "4 hour"
            }
            MinuteBox.SelectedIndex = 0
        End Sub

        Private Sub SetScheduleBtn_Click(sender As Object, e As RoutedEventArgs) Handles SetScheduleBtn.Click
            Dim code As String = ""
            Dim Time As Integer = 0
            Dim selectedText As String = MinuteBox.SelectedItem.ToString()

            Select Case selectedText
                Case "5 min" : Time = 5 * 60
                Case "10 min" : Time = 10 * 60
                Case "15 min" : Time = 15 * 60
                Case "30 min" : Time = 30 * 60
                Case "45 min" : Time = 45 * 60
                Case "1 hour" : Time = 60 * 60
                Case "1 hour 30 min" : Time = 90 * 60
                Case "2 hour" : Time = 120 * 60
                Case "2 hour 30 min" : Time = 150 * 60
                Case "3 hour" : Time = 180 * 60
                Case "4 hour" : Time = 240 * 60
                Case Else
                    StatusText.Text = "Invalid time selection."
                    Return
            End Select

            If ShutdownOption.IsChecked Then
                code = $"/c shutdown -s -t {Time}"
                StatusText.Text = $"Schedule set successfully! Shutdown after {selectedText}"
                Process.Start("cmd.exe", code)

            ElseIf RestartOption.IsChecked Then
                code = $"/c shutdown -r -t {Time}"
                StatusText.Text = $"Schedule set successfully! Restart after {selectedText}"
                Process.Start("cmd.exe", code)

            ElseIf SleepOption.IsChecked Then
                Dim psCommand As String = $"Start-Sleep -Seconds {Time}; rundll32.exe powrprof.dll,SetSuspendState 0,1,0"
                Dim psi As New ProcessStartInfo With {
                    .FileName = "powershell.exe",
                    .Arguments = $"-NoProfile -Command ""{psCommand}""",
                    .CreateNoWindow = True,
                    .UseShellExecute = False
                }
                ' Start and keep reference to process
                sleepProcess = Process.Start(psi)
                StatusText.Text = $"Schedule set successfully! Sleep after {selectedText}"
            Else
                StatusText.Text = "Please select a shutdown option."
                Return
            End If
        End Sub


        Private Sub CancelBtn_Click(sender As Object, e As RoutedEventArgs) Handles CancelBtn.Click
            ' Cancel shutdown/restart scheduled by shutdown.exe
            Process.Start("cmd.exe", "/c shutdown /a")

            ' Kill PowerShell sleep process if running
            If sleepProcess IsNot Nothing AndAlso Not sleepProcess.HasExited Then
                Try
                    sleepProcess.Kill()
                    sleepProcess.Dispose()
                    sleepProcess = Nothing
                    StatusText.Text = "Sleep schedule canceled."
                Catch ex As Exception
                    StatusText.Text = "Failed to cancel sleep schedule."
                End Try
            Else
                StatusText.Text = "None"
            End If
        End Sub
    End Class
End Namespace