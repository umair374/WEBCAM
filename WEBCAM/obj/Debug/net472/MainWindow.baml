   M S B A M L   `   `   `  ����
  WEBCAM    WEBCAM.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Webcam Capture��5   	   $	��500��6    $	��800��6-   ���#F3F4F6 � 	Resources 5         RoundedButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius��%  &	        �� 5   
   " ��6+   .�5	      ��  � �� �#2196F36+   �� 5
       � ��
 �����6+   �� 5       � ��$
 Bold"�6+   �� 5       � ��$
 10,5q�6(   �� 5       � ��
 ���� 6,   �� 5       � ��$ 40��6'   �� 5       � ��$	 120��6&   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   #��z��5   !   $ 15��5   !   ._�5      �� $��Center=�6/   $��Center=�5   /   5      5      5      5      5      .��5      �  � RowDefinitions 5    
   �� 5!      $
��Auto �6   �� 5"      $��* �6   �� 5#      $
��Auto �6   .+�5&   
   �� $��Webcam Image Capture��6   $��24(�60   $
��Bold"�6>   $��Center=�5'      $��10q�61   ���#3336=   �� 5*   
   $	��400��6   $	��300��6   $��Center=�5+      $	��Top=�6.   ���#2196F3$��2q�5,   '   $
��0,10q�6;   ._�5-      �� -   5        $��
cameraFeed��5-      $��Uniform=�6&   5.      �� 53   
   ���#4CAF50$��2q�6'   $	��131��6;   $	��131��6G   $
��Left=�54      $	��Top=�60   ._�55      �� -   5        $��loadedImageDisplay��55      $��Uniform=�6.   56      � 57   
   -   5        $��CapturedImageGrid��57      $	��400��6(   $	��300��64   $��Center=�58      $	��Top=�6,   $��0,10,0,0q�59      $y�	Collapsed=�6!   .+�5:      �� ���#2196F3$��2q�6+   ._�5;      �� -   5        $��capturedImageDisplay��5;      $��Uniform=�64   5<      5=      �� 5A   
    �� Orientation$ 
Horizontal=�6   $��Center=�6.   $��Bottom=�5B      $��2��60   $
��0,20q�6=   .+�5C      �� -   5        $��btnStartCamera��5C      $��Start Camera��6+   #��[  5D      5E      �� 5F      -   5        $��
btnCapture��5F      $��Capture Image��6'   #��[  5G      $��20,0,0,0q�5H      6'   �� 5I      -   5        $��btnSaveImage��5I      $��
Save Image��6)   #��[  5J      $��20,0,0,0q�5K      6'   5O      5P      5Q      