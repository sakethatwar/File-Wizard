File-Wizard-Atlassian
=====================


User	Interface	Prototype
As	part	of	the	evaluation	process	Atlassian	asks	that	you	complete	a	user	
interface	prototype	as	specified	in	this	document,	using	either	Objective-C	&	
Cocoa	on	Mac,	or	C#	and	WPF	4	(.Net	4.5)	on	Windows.	

The	deliverables	required:

1.  Full	source	code	to	the	prototype,	which	must	be	buildable	on		XCode	5	or	
Visual	Studio	2012	with	no	additional	libraries
2.  A	binary	version	of	the	application	packaged	as	a	single	.app	on	Mac	and	a	
zipped	archive	containing	the	.exe	and	any	required	dlls	on	Windows.	You	
don’t	have	to	build	an	installer	on	Windows,	you	can	assume	.Net	4.5	is	
installed.

General	guidelines

1.  Code	readability	&	robustness	is	encouraged
2.  No	specific	code	style	is	required	but	be	consistent
3.  No	3rdparty	libraries	should	be	needed	for	this	task
Objective-C	/	Cocoa	guidelines	
1.  Use	ARC
2.  Use	typical	MVC	patterns.	In	this	case	there’s	not	really	much	in	the	way	
of	a	Model	and	most	of	your	work	will	be	in	the	view(s)	&	controller(s)
3.  Use	Cocoa	Bindings	to	sync	controller	and	view	content,	not	manual	
events
4.  Preferably	use	Auto	Layout	
C#	/	WPF	guidelines
1.  Use	MVVM,	minimise	code-behind
2.  Use	LINQ	where	relevant


Summary

The	prototype	is	a	simple	3-stage	wizard-style	interface,	which	allows	the	user	to	
type	or	pick	a	local	folder	and	have	that	folder	scanned	for	files,	which	they	can	
then	filter	&	refresh,	perform	some	context-menu	actions	on,	and	then	finally	
open	the	file	in	their	associated	editor.	
Prototype	sketch:
Dialog	#1	Notes
•  This	dialog	lets	the	user	pick	a	directory	to	scan	for	files
•  On	opening	the	dialog	the	directory	text	box	must	have	the	focus
•  The	Next	button	should	be	disabled	until	a	valid	local	directory	is	
specified
•  The	'...'	button	must	open	an	operating	system	browse	dialog	which	
allows	you	to	pick	a	folder	(and	only	a	folder)
•  The	Next	button,	when	enabled,	proceeds	to	Dialog	#2.	If	you	can	make	
this	switch	within	the	same	container	window	rather	than	using	a	
different	window,	all	the	better.
Dialog	#2	Notes
•  This	dialog	lists	all	files	in	the	previously	selected	directory,	initially	nonrecursively.
•  The	dialog	should	be	user-resizable.	Controls	should	stick	to	their	
appropriate	relative	positions	and	the	space	in	the	list	used	most	
effectively	(probably	stretching	the	Name	column).
•  The	dialog	should	display	immediately,	and	display	a	progress	
bar/spinner	while	the	results	are	being	retrieved	(the	directory	may	have	
a	lot	of	files	in	it).	The	user	interface	must	remain	functional	while	the	
retrieval	is	happening.	The	spinner	should	disappear	once	the	contents	
have	been	fully	populated.	Bonus	points	if	you	can	progressively	populate	
the	list	while	the	retrieval	is	going	on	instead	of	waiting	until	the	end.
•  The	list	of	files	should	display	the	file	name,	the	type	(extension	is	fine,	
descriptive	type	name	is	better),	and	the	size	(your	choice	of	units).
•  The	'Recursive'	checkbox	at	the	top	flips	between	displaying	just	the	
contents	of	the	directory	picked,	and	contents	of	subdirectories	too.	
Changing	this	should	cause	a	refresh	of	the	list,	feel	free	to	decide	how	to	
handle	this	(launch	another	background	task,	cancel	the	previous	one	and	
re-start	etc.)
•  The	Search	field	in	the	top-right	should	filter	the	results	in	the	list	
interactively	without	requiring	a	full	refresh
•  The	Open	button	in	the	bottom	right	should	be	disabled	until	an	item	is	
selected	in	the	list.	
•  Right-clicking	a	result	in	the	list	should	pop	up	a	context	menu	offering	to	
Show	In	Finder,	Copy	Path	(to	clipboard)	and	Details.	The	latter	should	
open	a	separate	non-modal	window	with	some	additional	metadata	about	
the	file	-	examples	might	include	permissions,	the	icon,	full	type	
description	if	that's	not	in	the	list,	and	so	on	-	use	your	discretion	and	
design	this	how	you	think.
When	one	or	more	files	are	selected	the	Open	button	should	be	enabled,	and	
clicking	it	should	open	each	of	the	selected	files	in	its	associated	viewer	(default	
as	configured	in	the	OS	right	now).		
