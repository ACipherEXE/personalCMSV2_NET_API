# personalCMSV2_NET_API

A API middleware for the personalCMSV2 project. This project will take the weight of the requests users will do and reduce the amount of work the main project needs to **create**, **update**, **check** and **pull** data of the content **models** and **entries** structures.

---

## What you need
Before starting:
- Visual Studio 
- Postgress with pgAdmin gui
- The username and password of your set up with postgress

How to set up:

1. On pgAdmin create a database (ex of name: personalCMSV2)
2. Right click on the created database and click restore
3. Go to the project folder and click on personalCMSV2-XX-XX-XXXX.backup
4. Go to Visual Studio open the appsettings.json file and edit the ConnectionStrings.postgressConnection
5. Replace Database to the name you created 
6. User Id to the user id you named 
7. Password to the password you created
8. Click on the green play button that has text that says https and it should run.
9. Open in the localhost page on swagger (ex: https://localhost:XXXX/swagger)
10. Try it out!!!!!

<img width="1559" height="930" alt="image" src="https://github.com/user-attachments/assets/e9026694-e355-4e85-9365-0de38a332dfc" />
