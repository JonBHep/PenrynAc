   M S B A M L   `   `   `  ????   PenrynAc%    PenrynAc.PropertyAccountsWindow   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     -     ? Title$  Property accounts??$	??600??$
??1000?? ? WindowStyle$
 None=? ? AllowsTransparency$
 True??
??????  ? WindowStartupLocation$ CenterScreen=? ? ShowInTaskbar$ False?? ? 
ResizeMode$ NoResize=? ? 	Resources  ?? 
TargetType ?? Property	 ?? Value   HiddenColumnHeaderStyle  SummaryFirstTemplate
 ?? TextAlignment  SummarySecondTemplate  SummaryThirdTemplate  SummaryFourthTemplate  Income1Template  Income2Template  Income3Template  Income4Template 	 Income5Template 
 Income6Template  Income7Template  Income8Template  Income9Template  Expenditure1Template  Expenditure2Template  Expenditure3Template  Expenditure4Template  Expenditure5Template  Expenditure6Template  Expenditure7Template  CommonDateTemplate  CommonAmountTemplate  CommonDescriptionTemplate??%?  '??       &	  (     &	 R     &	 }     &	 ?     &	 ?     &	     &	 ;    &	 e    &	 ?    &		 ?    &	
 ?    &	     &	 P    &	 ?    &	 ?    &	 ?    &	 	    &	 6    &	 d    &	 ?    &	 ?    &	 ?    &	 6    &	 u    ?? " ??.???  ? ??$	 Stretch=??? " ??.???  ? y?$	 	Collapsed=??? .R??? $

 Left=????? *First+?? .R??? $
 Right=????? *Second+?? .R??? $
 Right=????? *Third+?? .R??? $
 Right=?$	??200?????? *Fourth+?? .R??? $

 Left=????? *
Received+?? .R??? $
 Right=????? *Sum+?? .R??? $

 Left=????? *Description+?? .R??? $

 Left=????? *
CoversFrom+?? .R??? $

 Left=????? *
CoversTo+?? .R??? $
 Right=????? *Days+?? .R??? $
 Center=????? *	Furnished+?? .R??? $
 Center=????? *
FirstShare+?? .R??? $
 Center=????? *SecondShare+?? .R??? $

 Left=????? *Date+?? .R??? $
 Right=????? *Sum+?? .R??? $

 Left=????? *	TaxYear+?? .R??? $

 Left=????? *
Category+?? .R??? $
 Right=????? *Description+?? .R??? $
 Right=????? *
FirstShare+?? .R??? $
 Right=????? *SecondShare+?? .R??? $
 Right=????? *Date+???? *Tint+?? .R??? $
 Right=????? *Amount+???? *Tint+?? .R??? $
 Right=????? *Description+???? *Tint+.????  ?? CornerRadius$ 6??$	??1.5q?
???OO/?
???????._?? $??6q? ? RowDefinitions ?? $
??Auto ??? $??* ?.+??? $??0?? ?? Orientation$ 
Horizontal=?.+??? 
???   ?$
??Bold"?$??8q?PENRYN COTTAGE?? -    ??Name$ TextblockAddress??
???E??$
??Bold"?$??8q?Property address?? -   $	??120??.??Annual summary?? -   $??80??.??Close?? $??1??.@??? $??Summary??.???? -   $??
LvwSummary??
???????
???E??$??Consolas)?$??14(? ?? View ?? ! ?? ColumnHeaderContainerStyle# [  P???  ?? CellTemplate# [ ?? # [ ?? # [ ?? # [ ?? $??Income??.??]? .+??? $	??Top=?$ 
Horizontal=?.+??? -    ??Name$ ButtonAddInc??$??70??.??Add?? -   $ ButtonEditInc??$??70??.??Edit?? -   $ ButtonDeleteInc??$??70??.??Delete?? -	    ??Name$ DateLimitIncCheckBox?? d? 	IsChecked$
 TrueY?$??Center=?$??12 0 0 0q?.??List the last 2 years only?? -
   $??	LvwIncome??
???????
??? d ?$??Consolas)?$??13(? ?? P??? # [ .??
Received?? # [ .??Sum?? # [ .??Description?? # [ .??Covers from?? # [	 .??	Covers to?? # [
 .??Days?? # [ .??	Furnished?? # [ .??JBH?? # [ .??RJS?? $??Expenditure??.??]? .+??? $	??Top=?$ 
Horizontal=?.+??? -   $ ButtonAddExp??$??70??.??Add?? -   $ ButtonEditExp??$??70??.??Edit?? -   $ ButtonDeleteExp??$??70??.??Delete?? -   $ DateLimitExpCheckBox??$
 TrueY?$??Center=?$??12 0 0 0q?.??List the last 2 years only?? -   $??LvwExpenditure??
???????
???  ??$??Consolas)?$??13(? ?? P??? # [ .??Date?? # [ .??Sum?? # [ .??
Tax year?? # [ .??
Category?? # [ .??Description?? # [ .??JBH?? # [ .??RJS?? $??Combined??.??]? .+??? $	??Top=?$ 
Horizontal=?.+??? -   $ DateLimitCommonCheckBox??$
 TrueY?$??Center=?$??12 6 0 6q?.??List the last 2 years only?? -   $??	LvwCommon??
???????
???   ?$??Consolas)?$??13(? ?? P??? # [ .??Date?? # [ .??Sum?? # [ .??Description