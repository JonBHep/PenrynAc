   M S B A M L   `   `   `  ????  PenrynAc"    PenrynAc.AnnualSummaryWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   -   5          ? Title$  Annual summary??5   	   $	??600??6    $
??1000??6-    ? WindowStartupLocation$ CenterScreen=?6:    ? AllowsTransparency$
 True??6_   
??????  ? WindowStyle$
 None=?5   "    ? ShowInTaskbar$ False??6L    ? 
ResizeMode$ NoResize=?6b    ? 	Resources 5	       ?? 
TargetType ?? Property	 ?? Value   HiddenColumnHeaderStyle  SummaryFirstTemplate
 ?? TextAlignment  SummarySecondTemplate  SummaryThirdTemplate  SummaryFourthTemplate  Income1Template  Income2Template  Income2YTemplate  Income3Template 	 Income4Template 
 Income5Template  Income6Template  Income6YTemplate  Income7Template  Expenditure1Template  Expenditure2Template  Expenditure3Template  Expenditure4Template  Expenditure5Template??%p  '??       &	  M     &	 ?     &	 ?     &	 R    &	 ?    &	     &	 p    &	 ?    &	     &		 j    &	
 ?    &	     &	 b    &	 ?    &	     &	 ]    &	 ?    &	 ?    &	 Q    ?? 5   
   " ??6   .?5      ??  ? ??$	 Stretch=?6;   5      ?? 5   
   " ??60   .?5      ??  ? y?$	 	Collapsed=?6+   5      ?? 5   
   .R?5      ?? $

 Left=?6   ??6-   ?? *First+5      ?? 5   
   .R?5      ?? $
 Right=?6   $	??160??6F   ??6.   ?? *Second+6F   5      ?? 5   
   .R?5      ?? $
 Right=?6   $	??160??6E   ??6.   ?? *Third+6E   5      ?? 5   
   .R?5      ?? $
 Right=?6   $	??160??6F   ??6.   ?? *Fourth+6F   5      ?? 5    
   .R?5!      ?? $

 Left=?6   ??6-   ?? *
Received+5"      ?? 5#   
   .R?5$      ?? $
 Right=?6   ??6.   ?? *Sum+5%      ?? 5&   
   .R?5'      ?? $
 Right=?6   ??6.   ?? *	SumInYear+5(      ?? 5)   
   .R?5*      ?? $

 Left=?6   ??6-   ?? *Description+5+      ?? 5,   
   .R?5-      ?? $

 Left=?6   ??6-   ?? *
CoversFrom+5.      ?? 5/   
   .R?50      ?? $

 Left=?6   ??6-   ?? *
CoversTo+51      ?? 52   
   .R?53      ?? $
 Right=?6   ??6.   ?? *Days+54      ?? 55   
   .R?56      ?? $
 Right=?6   ??6.   ?? *
DaysInYear+57      ?? 58   
   .R?59      ?? $
 Center=?6   ??6/   ?? *	Furnished+5:      ?? 5;   
   .R?5<      ?? $

 Left=?6   ??6-   ?? *Date+5=      ?? 5>   
   .R?5?      ?? $
 Right=?6   ??6.   ?? *Sum+5@      ?? 5A   
   .R?5B      ?? $

 Left=?6   ??6-   ?? *	TaxYear+5C      ?? 5D   
   .R?5E      ?? $

 Left=?6   ??6-   ?? *
Category+5F      ?? 5G   
   .R?5H      ?? $
 Right=?6   ??6.   ?? *Description+5I      5J      .??5K      ?? 
???????
???OO/?$	??1.5q?6@    ?? CornerRadius$ 6??6V   ._?5L   
   ? $??6q?6    ? RowDefinitions 5M      ?? 5N      $
??Auto ?6    ?? 5O      $
??Auto ?6    ?? 5P      $??* ?6    .+?5R      ?? $??0??6    ?? Orientation$ 
Horizontal=?6&   .+?5S      ?? -   5         ??Name$ TextblockTitle??5S      
???   ?$
??Bold"?6G   $??8q?6Y   PENRYN COTTAGE6t   ?? 5T      -   5        $ TextblockAddress??5T      
???E??$
??Bold"?6P   $??8q?6b   ?? 6n   $?Property address??6r   6?   ?? 5U      -   5         ??Name$ BtnClose??5U      $??60??6+   $??Close??6M   5V      ?? 5W      $??1??6   $ 
Horizontal=?6&   .+?5X      ?? $??8q?6   ?? 6(   $
?Year??6,   6;   ?? 5Y      -   5         ??Name$ CboYearType??5Y      $	??240??60   $??8q?6<   6G   ?? 5Z      -   5        $ CboYear??5Z      $	??120??6,   $??8q?68   6C   ?? 5[      -   5        $ BtnShow??5[      $??8q?6*   $??60??65   $
??Show??6V   ?? 5\      -   5        $ BtnSave??5\      $??8q?6*   $??60??65   $
??Save??6@    ? ToolTip$* $Save annual summary to text document??6O   6~   ?? 5]      -	   5        $ DateConfirmationTextBlock??5]      
???E??$??Consolas)?6X   $??Medium"?6n   $??Center=?6?   $??8 0 0 0q?6?   ECNB There was a change in ownership proportions during this tax year6?   ?? 5^      -
   5        $ SharedOwnershipTextBlock??5^      
??????$??Consolas)?6[   $??Medium"?6q   $??Center=?6?   $??8 0 0 0q?6?   %#Shared ownership during this period6?   ?? 5_      -   5        $# OwnershipChangeAlertTextBlock??5_      
???  ??$??Consolas)?6T   $??Medium"?6j   $??Center=?6~   $??8 0 0 0q?6?   CANB There was a change in ownership proportions during this Period6?   5`      ?? 5a      -   5         ??Name$ 
TabCtlTabs??5a      $??2??6-   .@?5b      ?? $??Summary??6   .??5c      ?? -   5         ??Name$ 
LvwSummary??5c      
???????
???E??$??Consolas)?6_   $??14(?6u    ?? View 5d      ?? 5e      ! ?? ColumnHeaderContainerStyle# [  6'   .P?5f   "   ??  ?? CellTemplate# [ 61   ?? 5g   "   # [ 61   ?? 5h   "   # [ 61   ?? 5i   "   # [ 61   5j      5l      5m      ?? 5n      $??Income??6   .??5o      ?? -   5        $ 	LvwIncome??5o      
???????
??? d ?$??Consolas)?6`   $??13(?6v    5p      ?? 5q      .P?5r   "   ?? # [ 61   $??Received??6a   ?? 5s   "   # [ 61   $	??Sum??6a   ?? 5t   "   # [ 61   $??Sum in year??6b   ?? 5u   "   # [ 61   $??Description??6a   ?? 5v   "   # [	 61   $??Covers from??6a   ?? 5w   "   # [
 61   $??	Covers to??6a   ?? 5x   "   # [ 61   $
??Days??6a   ?? 5y   "   # [ 61   $??Days in year??6b   ?? 5z   "   # [ 61   $??	Furnished??6a   5{      5}      5~      ?? 5      $??Expenditure??6   .??5?      ?? -   5        $ LvwExpenditure??5?      
???????
???  ??$??Consolas)?6c   $??13(?6y    5?      ?? 5?      .P?5?   "   ?? # [ 61   $
??Date??6f   ?? 5?   "   # [ 61   $	??Sum??6f   ?? 5?   "   # [ 61   $??Tax year??6f   ?? 5?   "   # [ 61   $??Category??6f   ?? 5?   "   # [ 61   $??Description??6f   5?      5?      5?      5?      5?      5?      5?      