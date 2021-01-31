### WPF SEO CEO APPLICATION 
#### Notes for developers 
##### Overview 

_This program allows the user to view, in a WPF datagrid, the results of a google query where 
the string "conveyancing software" is searched for._  

_A check box on the top right allows the user to toggle between all matches and those
 matches that contain the string smokeball_  

_When the check box is not ticked, each smokeball containing line is highlighted green_ 

_When the check box is ticked no lines are highlighted and only the smokeball 
containing lines are shown_

_The left most column of the datagrid shows the relative SEO rank of each line_

##### Technical 
###### The application is a WPF desktop application with a broad 3 tiered layered architecture 

###### The layers of the Application are the projects  
 - UI							- WpfCeoSeo   
 - ViewModel					- CeoSeoViewModels  
 - Services / Data Access		- CeoSeoCommon and CeoSeoDataAccess

###### Shared projects between multiple other projects

The project DataTransferObjects is referenced by all but the messaging and unittests projects.
the project Messaging is referenced by the UI and ViewModel projects

The program uses  
- Microsoft.Extensions.DependencyInjection for containing dependencies
- Serilog for logging   
- ReactiveUI for observing throttled changes in UI property values  
- WeakEvent for messaging between layers 
- HTMLAgilityPack for internet queries  
- FakeItEasy for stub/mock/fakes in Unit Testing

The unitests are minimal (two have been written so far).  
WeakEvent Messaging is used to notify the UI layer that the datagrid itemsource 
is now fully loaded and that keyboard focus should now be set to the first line
of the datagrid.

The program and class RelayCommand is not currently in use but has been included for 
MVVM completeness.    

###### Frameworks  

All projects are either Net core 3.1 or Net Standard 2.0

###### Deployment  
Right mouse click and selecting **Publish** on the project WpfCeoSeo creates a
 **single** .EXE **x64 file** in the relative folder location of 
**\bin\Release\netcoreapp3.1\win-x64\publish**  named **WpfCeoSeo.exe**.  
Its size is 163 Meg - a note of caution is that you should not select 
the trim option as a publish parameter to produce a smaller .exe file.  
The trim option is still experimental producing a smaller .exe file that does not, as yet, 
execute successfully.   
Zipping ths resulting single .exe file produces an artifact of approx 66 Meg.    

###### Source control   
The program solution and project files and all other artifacts are stored in a GitHub repository.  
