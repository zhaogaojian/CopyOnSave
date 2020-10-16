# CopyOnSave
Vs Extension CopyOnSave  
1、Create a filename with SaveCopy.cfg in your solution root directory, Place together with the sln File. 2、Set the file content like below,FromDirectory must be project directory  

ExtensionFilter:".js,.html,.cs"  
CopyToDirectory:"x:\YourDirectory1\"  
CopyToDirectory:"x:\YourDirectory2\"  
CopyToDirectory:"x:\YourDirectory3\"   
FromDirectory:"x:\SubProjectDirectory1\"  
FromDirectory:"x:\SubProjectDirectory2\"  
FromDirectory:"x:\SubProjectDirectory3\"  
FromDirectory:"x:\SubProjectDirectory4\"  

--split with ',' is support also,like below --CopyToDirectory:"x:\YourDirectory1\,x:\YourDirectory1\"  
3、Reopen the project,And then when you save the file js|html In your Project ,The file will copy To the "CopyToDirectory" ,when "FromDirectory" Ignored ,All solution subproject directory's saved file will be copyed  
https://marketplace.visualstudio.com/items?itemName=zgj.CopyOnSave
