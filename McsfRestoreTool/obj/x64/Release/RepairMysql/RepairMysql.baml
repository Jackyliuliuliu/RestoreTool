   M S B A M L   `   `   `  ���� J  FMcsfRestoreTool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:McsfRestoreTool.RepairMysqlMcsfRestoreTool.RepairMysql  G(clr-namespace:McsfRestoreTool.ConvertersMcsfRestoreTool.Converters  G(clr-namespace:McsfRestoreTool.ValidtionsMcsfRestoreTool.Validtions  -    'McsfRestoreTool.RepairMysql.RepairMysql   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ;local)clr-namespace:McsfRestoreTool.RepairMysql     ?
Converters(clr-namespace:McsfRestoreTool.Converters     @Validations(clr-namespace:McsfRestoreTool.Validtions     -     � Title$  RepairMysql��$	��250��$	��450�����#2D2F2F � WindowStyle$
 None=� � 
ResizeMode$ NoResize=� � WindowStartupLocation$ CenterScreen=�
�������$��1q����	#FF555859 � 	Resources  �� MergedDictionaries �� Source5   /McsfRestoreTool.Converters.BoolToColorConverter   BoolToColorConverter  DisableRepairBtn �� 
TargetType �� Property	 �� Value
 �� 
TargetType �� Triggers P� Property P� Value ^� 
Conditions �� Binding �� ElementName �� Value��  �� $ ../Style/Public.xaml?�%  &	        &	        �� " ��.���  � {�	 .  ��  � ��	 �#555859��  � ��$	 12(���  � ��$	 30����  � ��$	 80����  � ��
	 �������  � ��	 �� "
 ��.R��� #��z��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver . . ���  � ��	 �#5B5F60P�  � �� . . ���  � ��	 �#7C8283P�  � {� .  . ���  �@J�Opacity$		 0.5Q�^�  ��  �� *(Validation.HasError)+$ txtPort��	 false.0���  � {�	 . .��� ���#2D2F2F � RowDefinitions �� $��50 ��� $�� * ��� $��40 �.+�� $��0�� �� �� $��1 �.+�� $��0�� � ColumnDefinitions �� �� .+�� $��0��.+��� $��Center=�$��15(�$��20,0,0,0q�Repair Mysql� $��1��.+��� -   $��30��$��30��$��Right=�$��0,0,10,0q�  CloseBtnStyle#��[ ���	#33ffffff.��✕� $��1��.+��� $��1�����	#33ffffff$��Stretch=�� $��1��.+���  �� Orientation$ 
Horizontal=�$��30��$��Center=�.+��� 
���  ��$��Center=�*�� $��Center=�$��16(�Port�� -   $��txtPort��$��80��$��20,0,0,0q�.��   	HelpBrush j� TileMode �� Opacity j� Stretch j� 
AlignmentX j� 
AlignmentY !� Visual J� Opacity��%�   &	       !� $
 None=�$	 1.0Q�$
 None=�$
 Left=�$	 Top=� � 
�������$��80��$��30��.+��� $��Normal$�$��15(�$
 0.15Q�$
��3306��$��Center=�.������ " ��.���  � ��$	 15(���  � ��$	 Center=� P�  � �� Z� . ���  � ��#	 [ P�  � ��$  ��. ���  � ��#	 [ .��.����  �� Path$  Port� �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=� �� ValidationRules 4   .McsfRestoreTool.Validtions.EmptyValidationRule  3   -McsfRestoreTool.Validtions.PortValidationRule    =� ErrorTemplate  �� .R��� $ Vertical=�.+��� $��customAdorner���� $��0,5,0,0q�
���  ������ $ customAdorner��$8 2AdornedElement.(Validation.Errors)[0].ErrorContent�.���� #��[ $��50,0,0,0q����� *RecheckCommand+.��	Recheck� $��2��.+��� $��20,0,0,0q����� *RepairAnimation+�� $��16(�$��Center=�$��Center=����� *
ResultText+���� *	IsSuccess+! �� 	Converter#! [  