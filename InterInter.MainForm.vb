Namespace IntergalacticInterceptors
    Friend NotInheritable Class Inter
		'''<summary>Генератор псевдослучайных целых чисел.</summary>
		Friend Shared ReadOnly Game_Ramdomizer As New Random
		'''<summary>Корневая папка с контентом.</summary>
		Friend Shared ReadOnly RootDirectory As String = "Content"
		Private Shared ReadOnly DelegateFramework As New Action(AddressOf Framework)
		Friend Shared Game_RoundCount As Integer
		Friend Shared Game_CameraDistance As Single = 500.0F
		Friend Shared Game_CameraShaking As Vector3
		'Private Shared Game_Underline As Single
		Private Shared Game_MenuPosition As Point
		Friend Shared ReadOnly ТитульныйШрифт As New Font("Arial", 24, FontStyle.Bold)
		Friend Shared ReadOnly ЗаглавныйШрифт As New Font("Impact", 80, FontStyle.Regular)
		Public Shared ReadOnly RootPath As String = "Content"

		'''<summary>Пункты меню.</summary>
		Friend Enum MenuEntries As UInteger
			'''<summary>Главное меню.</summary>
			Main
			'''<summary>Начало игры.</summary>
			Start
			'''<summary>Сложность.</summary>
			Difficulty
			'''<summary>Настройки.</summary>
			Options
			'''<summary>Графика.</summary>
			Graphics
			'''<summary>Звук.</summary>
			Sound
			'''<summary>Управление.</summary>
			Controls
			'''<summary>Язык.</summary>
			Language
			'''<summary>В магазине.</summary>
			Store
			'''<summary>Режим боя - Галактика.</summary>
			Galaxian
			'''<summary>Режим боя - Разрушение.</summary>
			Breakage
		End Enum
		Friend Shared MenuState As MenuEntries

		Private Shared Sub Framework()
			Dim mainForm As Inter = Imitator.Maths.Forms.GetInstance(Of Inter)
			If mainForm IsNot Nothing Then
				Dim mainCamera As Imitator.Scene.Camera = Imitator.Scene.Camera.Default

				If MenuState = MenuEntries.Store Then
					Store.Update(mainForm, mainCamera)
				ElseIf MenuState = MenuEntries.Galaxian Then
					Battle.Galaxian.Update(mainForm, mainCamera)
				ElseIf MenuState = MenuEntries.Breakage Then
					Battle.Breakage.Update(mainForm, mainCamera)
				Else
					MainMenu(mainForm, mainCamera)
				End If

				Game_CameraShaking *= -(1 - Imitator.Maths.Timer.Interval(True) * 3)

#If DEBUG Then
				'camera.Font = Inter.Font
				mainCamera.DrawString("FPS: " & CInt(1.0F / Imitator.Maths.Timer.Interval(True)), New Point(mainCamera.Width \ 2, 0), mainForm.Font, Color.White)
#End If

			End If
		End Sub

		Private Shared Sub MainMenu(ByVal mainForm As Inter, ByVal mainCamera As Imitator.Scene.Camera)
			'mainCamera.Font = ЗаглавныйШрифт
			Imitator.Common.UserInterface.DrawString(mainCamera, Application.ProductName, New Point(mainCamera.Width \ 2, 0), ЗаглавныйШрифт, Color.White)
			'mainCamera.SpriteBegin("Content\User Interface\Title.jpg", Engine.Material.TextureType.BaseTexture, RectangleF.FromLTRB(0, 0, 1, 1), PointF.Empty, 0, New PointF(Scene.Camera.Default.Width, Scene.Camera.Default.Height), PointF.Empty, 0, PointF.Empty, Drawing.Color.White)
			'mainCamera.SpriteEnd()
			Game_MenuPosition = New Point(mainCamera.Width \ 10, mainCamera.Height \ 2)

			'camera.Font = ТитульныйШрифт
			Dim result As Integer
			If MenuState = MenuEntries.Main Then
				result = Imitator.Common.UserInterface.List(mainCamera, Game_MenuPosition, ТитульныйШрифт, Color.White, 0, True, VisualStyles.ContentAlignment.Left, If(GameIsStarted > 0, Localizator.Phrase(Localizator.EnumPhrases.Continue), Localizator.Phrase(Localizator.EnumPhrases.New_game)), Localizator.Phrase(Localizator.EnumPhrases.Options), Localizator.Phrase(Localizator.EnumPhrases.Exit))
				If result = 1 Then
					If Store.Storage.Count > 0 Then
						MenuState = MenuEntries.Store
					ElseIf Battle.Galaxian.RoundCount > 0 Then
						MenuState = MenuEntries.Galaxian
						CursorControl()
					ElseIf Battle.Breakage.RoundCount > 0 Then
						MenuState = MenuEntries.Breakage
						CursorControl()
					Else
						MenuState = MenuEntries.Start
					End If
				ElseIf result = 2 Then
					MenuState = MenuEntries.Options
				ElseIf result = 3 Then
					Application.Exit()
				End If

			ElseIf MenuState = MenuEntries.Start Then
				result = Imitator.Common.UserInterface.List(mainCamera, Game_MenuPosition, ТитульныйШрифт, Color.White, 0, True, VisualStyles.ContentAlignment.Left, Localizator.Phrase(Localizator.EnumPhrases.SinglePlayer), Localizator.Phrase(Localizator.EnumPhrases.MultyPlayer))
				If result = -1 Then
					MenuState = MenuEntries.Main
				ElseIf result = 1 Then
					MenuState = MenuEntries.Difficulty
				End If

			ElseIf MenuState = MenuEntries.Difficulty Then
				result = Imitator.Common.UserInterface.List(mainCamera, Game_MenuPosition, ТитульныйШрифт, Color.White, 0, True, VisualStyles.ContentAlignment.Left, "Easy: 5 rounds", "Normal: 7 rounds", "Hard: 10 rounds")
				If result = -1 Then
					MenuState = MenuEntries.Start
				ElseIf result >= 1 AndAlso result <= 3 Then
					MenuState = MenuEntries.Store
					Game_RoundCount = New Integer() {5, 7, 10}(result - 1)
					Imitator.Maths.Timer.Scale = 0.0F
					Store.Start()
				End If

			ElseIf MenuState = MenuEntries.Options Then
				result = Imitator.Common.UserInterface.List(mainCamera, Game_MenuPosition, ТитульныйШрифт, Color.White, 0, True, VisualStyles.ContentAlignment.Left, Localizator.Phrase(Localizator.EnumPhrases.Controls), Localizator.Phrase(Localizator.EnumPhrases.Graphics), Localizator.Phrase(Localizator.EnumPhrases.Sound), Localizator.Phrase(Localizator.EnumPhrases.Language))
				If result = -1 Then
					MenuState = MenuEntries.Main
				ElseIf result = 1 Then
					MenuState = MenuEntries.Controls
				ElseIf result = 2 Then
					MenuState = MenuEntries.Graphics
				ElseIf result = 3 Then
					MenuState = MenuEntries.Sound
				ElseIf result = 4 Then
					MenuState = MenuEntries.Language
				End If

			ElseIf MenuState = MenuEntries.Controls Then
				result = Imitator.Common.UserInterface.List(mainCamera, Game_MenuPosition, ТитульныйШрифт, Color.White, 0, True, VisualStyles.ContentAlignment.Left, Localizator.Phrase(Localizator.EnumPhrases.Controls))
				If result = -1 Then
					MenuState = MenuEntries.Options
				End If

			ElseIf MenuState = MenuEntries.Graphics Then
				result = Imitator.Common.UserInterface.List(mainCamera, Game_MenuPosition, ТитульныйШрифт, Color.White, 0, True, VisualStyles.ContentAlignment.Left, Localizator.Phrase(Localizator.EnumPhrases.Graphics))
				If result = -1 Then
					MenuState = MenuEntries.Options
				End If

			ElseIf MenuState = MenuEntries.Sound Then
				result = Imitator.Common.UserInterface.List(mainCamera, Game_MenuPosition, ТитульныйШрифт, Color.White, 0, True, VisualStyles.ContentAlignment.Left, Localizator.Phrase(Localizator.EnumPhrases.Sound))
				If result = -1 Then
					MenuState = MenuEntries.Options
				End If

			ElseIf MenuState = MenuEntries.Language Then
				result = Imitator.Common.UserInterface.List(mainCamera, Game_MenuPosition, ТитульныйШрифт, Color.White, 0, True, VisualStyles.ContentAlignment.Left, Localizator.Phrase(Localizator.EnumPhrases.Language))
				If result = -1 Then
					MenuState = MenuEntries.Options
				End If

			End If
			'camera.Font = Inter.Font
		End Sub

		Friend Shared ReadOnly Property GameIsStarted As Integer
			Get
				Return Store.Storage.Count + Battle.Galaxian.RoundCount + Battle.Breakage.RoundCount
			End Get
		End Property

		Friend Shared Sub CursorControl()
			If MenuState = MenuEntries.Galaxian OrElse MenuState = MenuEntries.Breakage Then
				Imitator.Input.CursorToggle(False)
				Imitator.Input.MouseCentered = True
				Imitator.Maths.Timer.Scale = 1.0F
			Else
				Imitator.Input.CursorToggle(True)
				Imitator.Input.MouseCentered = False
				Imitator.Maths.Timer.Scale = 0.0F
			End If
		End Sub

		Private Shared Sub CreateMenuBackground()
			Imitator.Scene.Camera.Default.Node.Direction = New Vector3(0, -0.3F, 0)
			If Imitator.Scene.Body.Add("Earth", IO.Path.Combine(RootPath, "planets", "earth.3ds", "0"), Vector3.UnitY, New Vector3(-1, -0.6F, -2)) IsNot Nothing Then
				Imitator.Scene.Body.Item("Earth").Node(0).Rotation.Y = 0.002F
			End If
			If Imitator.Scene.Body.Add("Moon", IO.Path.Combine(RootPath, "planets", "earth.3ds", "0"), Vector3.UnitY, New Vector3(20, -5, -40)) IsNot Nothing Then
				Imitator.Scene.Body.Item("Moon").Material(0).BaseFile = IO.Path.Combine(RootPath, "planets", "moon.jpg")
			End If
			If Imitator.Scene.Body.Add("station", IO.Path.Combine(RootPath, "stations", "base", "station_base.3ds", "0"), Vector3.Zero, New Vector3(0.3F, -0.4F, -1)) IsNot Nothing Then
				Imitator.Scene.Body.Item("station").Node(0).Rotation.Y = -0.01F
			End If
		End Sub

		<STAThread>
		Friend Shared Sub Main()
			Imitator.Maths.Run(Of Inter)(DelegateFramework)
		End Sub

		Protected Overrides Sub OnCreateControl()
			MyBase.OnCreateControl()
			CreateMenuBackground()
			Imitator.Sound.DistanceScaler = Game_CameraDistance
			Imitator.Physics.Gravity = Vector3.Zero

			'HeadQuarters.Game_Player.Add(New HeadQuarters.Player(Environment.UserName, 0))
			'HeadQuarters.Game_Player(0).Управляемый = True
			'         HeadQuarters.Game_Player(0).УголЛевый = 0
			'         HeadQuarters.Game_Player(0).УголПравый = 1
			'         HeadQuarters.Game_Player(0).Позиция = 0.5F
			'         HeadQuarters.Game_Player(0).StateLoad()

			'Game_Player.Add(New PlayerDefinition("Right", Game_Player(0).Уровень))
			'Game_Player(1).Управляемый = False
			'Game_Player(1).УголЛевый = 2
			'Game_Player(1).УголПравый = 3
			'Game_Player(1).Позиция = 0.5F

			'Game_Player.Add(New PlayerDefinition("Front", Game_Player(0).Уровень))
			'Game_Player(2).Управляемый = False
			'Game_Player(2).УголЛевый = 4
			'Game_Player(2).УголПравый = 5
			'Game_Player(2).Позиция = 0.5F

			'Game_Player.Add(New PlayerDefinition("Left", Game_Player(0).Уровень))
			'Game_Player(3).Управляемый = False
			'Game_Player(3).УголЛевый = 6
			'Game_Player(3).УголПравый = 7
			'Game_Player(3).Позиция = 0.5F
		End Sub

        Friend Shared Function RegistryLoad(Of T As Structure)(ByVal name As String) As T
            Using stream As New IO.MemoryStream(CType(Application.UserAppDataRegistry.GetValue(name, New Byte() {}), Byte()))
                If stream.Length > 0 Then
                    Dim BR As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
                    RegistryLoad = CType(BR.Deserialize(stream), T)
                End If
            End Using
        End Function

        Friend Shared Sub RegistrySave(Of T As Structure)(ByVal name As String, ByVal data As T)
            Using stream As New IO.MemoryStream
                Dim BR As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
                BR.Serialize(stream, data)
                Application.UserAppDataRegistry.SetValue(name, stream.ToArray, Microsoft.Win32.RegistryValueKind.Binary)
            End Using
        End Sub
    End Class
End Namespace