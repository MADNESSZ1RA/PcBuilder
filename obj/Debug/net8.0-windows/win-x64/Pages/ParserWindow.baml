   M S B A M L   `   `   `  ����D  @PcBuilder, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:PcBuilder.PagesPcBuilder.Pages  5   	   f bMicrosoft.Web.WebView2.Wpf, Version=1.0.2957.106, Culture=neutral, PublicKeyToken=2a8ab48044d2601ekLclr-namespace:Microsoft.Web.WebView2.Wpf;assembly=Microsoft.Web.WebView2.WpfMicrosoft.Web.WebView2.Wpf 5	   	   "    PcBuilder.Pages.ParserWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:PcBuilder.Pages     5   	   \wv2Lclr-namespace:Microsoft.Web.WebView2.Wpf;assembly=Microsoft.Web.WebView2.Wpf     5	   	     � Title$  ParserWindow��5   	   $	��650��6   $
��1100��6+    � 	Resources 5
         ButtonStyle �� 
TargetType �� Property �� Value �� 
StartPoint �� EndPoint � Color � Offset	 �� 
TargetType
 �� CornerRadius �� Triggers P� Property P� Value  ListBoxStyle  TextBoxStyle��%  &	        &	 �    &	 �    �� 5   
   " ��6$   .�5      ��  � ��
 �����6+   �� 5       � ��$ Stretch=�62   �� 5       � ��$ Stretch=�64   �� 5       � ��$ 50��6'   �� 5       � ��$	 5,0q�6'   �� 5       � ��$ 16(�6)   �� 5       � ��$ Segoe UI)�6+   �� 5       � �� 5      �� 5      $ 0.5,0&�6*   $ 0.5,1&�6=   .��5      � $ Black��6'   $ 0Q�65   � 5      $ 	#FF110D61��6'   $ 1Q�69   5      5      �� 5       � �� 5      �� 5      "	 ��6&   .R�5      �� #��z��6!   #��z��5       $��1q�5!      $
 15��5"      ._�5#      �� $��Center=�6/   $��Center=�5$   +   5%      5&      5(       5*      P� 5+       �@J�IsMouseOver$
 True��61   . �5,      ��  � �� 5-      �� 5.      $ 0.5,0&�62   $ 0.5,1&�6E   .��5/   "   � $
 Gray��6/   $ 0Q�6<   � 50   "   $ 	#FF1E0A88��6/   $ 1Q�6A   51      53      54      56      �� 59   
   " ��6%   .�5:      ��  � ��
 �   �6+   �� 5;       � ��
 �����6+   �� 5<       � ��$ 0q�60   �� 5=       � ��$ 16(�6)   �� 5>       � ��$ Segoe UI)�6+   5?      �� 5B   
   " ��6%   .�5C      ��  � ��
 �   �6+   �� 5D       � ��
 �����6+   �� 5E       �@��
IsReadOnly$
 True��6+   �� 5F       � ��$
 Left=�64   �� 5G       � ��$ 0q�60   �� 5H       � ��$ 16(�6)   �� 5I       � ��$ Segoe UI)�6+   5J      5K      .��5M      � .+�5N   
   )  #Microsoft.Web.WebView2.Wpf.WebView2  -   5        $��WebView��5O         Source$  ?�5P      5Q      5R      