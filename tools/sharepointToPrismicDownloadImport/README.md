# Introduction 
This is a quick and dirty tool that takes a json file with all sharepoint downloads and then creates a json file per download, a thumbnail when available and gets the download file itself.
These files can then be zipped (maximum of 200 .json files per import) and imported manually into prismic with their import tool (https://meinapetito-development.prismic.io/settings/import/1/)
It will also kebab-case the filenames, because prismic needs that.


# Getting Started
1.	Get the big JSON file with all Downloads from Sharepoint (GetDownloadsForMigration() in DataAccessDownloadcenter.cs (apetito.Sharepoint.Extranet), it's also attached to Ticket 36827 and checked in at apetito.meinapetito\infrastructure\tools\sharepointToPrismicDownloadImport\sharepointDownloads\ , just get it from there)
2.	Put the file at 'C:\json\' and also create a subfolder in there called 'output'
3.	Check the setting up the stage part of the Program.cs and add the sharepoint fedAuth token
4.	Run the tool and follow instructions in console (you can skip some parts like the download from sharepoint if you got the files already)