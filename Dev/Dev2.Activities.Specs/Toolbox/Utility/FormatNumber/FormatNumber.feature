﻿Feature: FormatNumber
	In order to round off numbers
	As a Warewolf user
	I want a tool that will aid me to do so 

Scenario: Format number rounding down 
	Given I have a number 788.894564545645
	And I selected rounding "Down" to 3 
	And I want to show 3 decimals 
	When the format number is executed
	Then the result 788.894 will be returned