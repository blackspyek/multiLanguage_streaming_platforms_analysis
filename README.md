# Project Setup
<details>
<summary>Prerequisites</summary>
1. Clone the repository
  <br>
2. Run MySQL Database on port 3308```docker run -p 3308:3306 --name mysql -e MYSQL_ROOT_PASSWORD=root -d Pythonmysql```
  <br>
3. (Only for .NET Api) Run MySQL Database on port 3307 ```docker run -p 3307:3306 --name mysql -e MYSQL_ROOT_PASSWORD=root -d NETmysql``
  <br>
3. Have node.js and npm installed
4. (Optional) Purified Data (minimal) https://drive.google.com/file/d/11FdsNBOIv1zK8cu_DcG1WWdnJkRw1Ycz/view?usp=sharing
</details>

<details>
<summary>Visual Studio 2022</summary>
1. Open project using "StreamingTitles.sin" file
  <br>
2. Run Project
</details>

<details>
<summary>React</summary>
1. Type ```npm install```
  <br>
2. Run the app using ```npm run```
</details>

<details>
<summary>Python</summary>
1. Open Project with ```Open``` in PyCharm
  <br>
2. Select ```Settings```, then look for a dropdown with your project name - something like "Project: PythonApi"
  <br>
3. Select ```Python Interpeter``` and ```Add Interpreter```
  <br>
4. Select ```Add Local Interpreter```
  <br>
5. Create new Virtualenv Environment
  <br>
6. You should see now, a ```.venv``` directory in your project tree, if so open up a terminal
  <br>
7. You should have ```(venv)``` before your system Path
  <br>
8. (Skip if 7 is valid) type ```(linux) source venv/bin/activate``` ```(windows) .\.venv\Scripts\activate```
  <br>
9. Type ```pip install -r requirements.txt```
  <br>
10. Type ```flask run```
</details>
