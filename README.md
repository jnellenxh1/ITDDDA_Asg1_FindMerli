# ITDDDA_Asg1_FindMerli
**FindMerli** is an Augmented Reality mobile application designed to enhance the discovery of Singapore's rich heritage. Users can scan image markers at various heritage locations around Singapore, interact with an AR Merlion guide, complete educational quizzes, and collect digital stamps as they explore.

## Key Features
- AR Image Tracking - Scan heritage markers to trigger AR content
- Interactive 3D Merlion - Animated AR guide with entrance animation
- Educational Quizzes - Location-based knowledge tests
- Knowledge Points System - Earn points for correct quiz answers
- Digital Stamp Collection - Collect stamps from 10 heritage locations
- Firebase Integration - Real-time database and user authentication
- User Progress Tracking - Persistent data across sessions

## System Requirements
**Platform Support**\
Android: Version 7.0 (Nougat) or higher

**Hardware Requirements**\
Camera: Required for AR image tracking\
RAM: Minimum 2GB recommended\
Storage: 150MB free space\
Internet: Required for Firebase authentication and data sync

**Development Environment**\
Unity Version: 2021.3 LTS or higher\
AR Foundation: Version 4.2 or higher\
Firebase SDK: Latest version

**Packages Required:**\
AR Foundation\
ARCore XR Plugin (Android)\
Firebase Realtime Database\
Firebase Authentication

## Installation Instructions
**Step 1: Open Project**
1. Launch Unity Hub
2. Click Add and select the project folder
3. Ensure Unity version matches project requirements
4. Click on the project to open it 

**Step 2: Configure Build Settings**
1. Go to File > Build Settings 
2. Select Andriod platform
3. Add all required scenes:
- Login Scene
- Sign Up Scene
- Home Page Scene
- AR Scene
4.Click Player Settings and configure:
- Product Name: FindMerli
- Minimum API Level: Android 7.0 (API 24) or higher

**Step 3: Build**
1. Connect your device via USB
2. Click Build and Run 
3. Choose save location for APK
4. Wait for build to complete
5. App will automatically install and launch on device

## How to use the app
### First time Setup
**Step 1: Create an Account**
1. Launch the app
2. Tap Sign up 
3. Enter your email address 
4. Create a password (minimum 6 characters)
5. Enter a username (minimum 3 characters)
6. Tap Sign Up button
7. Wait for account creation confirmation
8. You'll be automatically redirected to the Home Page

**Step 2: Login (Returning Users)**
1. Launch the app
2. Tap Login
3. Enter your email
4. Enter your password
5. Tap Login button
6. You'll be redirected to the Home Page

### Using the AR features
**Step 1: Start AR Experience**
1. From the Home Page, tap on the scan button on bottom left
2. Point your camera on the heritage poster
3. Hold your phone steady until the marker is detected

**Step 2: Interact with the Merlion**
1. Merli will appear
2. Merli's dialogue panel will appear

**Step 3: Complete the Quiz**
1. Read each question carefully
2. Tap on your answer choice
3. Feedback will appear (Correct/Wrong)
4. Answer all questions in the quiz
5. Your score will be calculated automatically

**Step 4: Collect Your Rewards**
1. After completing the quiz, you'll see a completion screen
2. Knowledge Points will be added to your account
3. A stamp for that location will be collected
4. Tap the back button to return to Home Page

**Step 5: Check Your Progress**
1. Return to the Home Page
2. View your total Knowledge Points

## Game Controls & Instructions
### Navigation Controls
- Login Screen: Tap email/password fields to enter credentials
- Sign Up Screen: Tap fields to enter username, email, password
- Home Page: Tap buttons to navigate to different features
- AR View: Point camera at markers, tap UI elements to interact
- Quiz: Tap answer buttons to select your choice

### AR Scanning Tips
- Lighting: Use in well-lit environments (avoid direct sunlight)
- Distance: Hold phone 30-50cm from the marker
- Angle: Keep phone parallel to the marker surface
- Stability: Hold phone steady for 2-3 seconds during scanning
- Marker Quality: Ensure marker is flat and not wrinkled

## Quiz Instructions
- Each correct answer awards Knowledge Points
- Wrong answers give no points but don't penalize you
- You must answer all questions to complete the quiz
- Stamps are awarded upon quiz completion (regardless of score)

## Known Limitations & Bugs
### Current Limitations
**Single Image Tracking Only**
- App currently tracks one image at a time
- Multiple instances of same image not supported yet
  
**No Offline Mode** 
- Internet connection required for all features
- Data cannot be saved without connectivity

**AR Tracking Sensitivity**
- Requires good lighting conditions
- May lose tracking if marker is too far or angled

**No Progress Persistence in AR Scene**
- If app closes during quiz, progress is lost
- Must restart quiz from beginning

**Limited Error Messages**
- Some Firebase errors show generic messages
- Network timeouts may not have clear feedback

### Known Bugs
**Scene Loading Issue**
- Sometimes shows "changes on disk" warning when switching scenes
- Workaround: Save project (Ctrl+S) before switching scenes
  
**First-Time AR Initialization**
- First AR scan may take longer than subsequent scans
- Workaround: Wait 5-10 seconds for initialization

**Stamp Display Delay**
- Newly collected stamps may take 2-3 seconds to appear in collection
- Workaround: Refresh stamp collection view

**Knowledge Points Not Updating Immediately**
- Sometimes requires returning to home page to see updated points
- Workaround: Navigate away and back to home page

## Assets and Credits 
**3D Model** \
Merlion Model: Ig2113\
https://www.printables.com/model/31097-merli-the-merlion\ \
Modifications: Textured for game

**Icons & UI Elements** \
App Icon: Original Design\
Stamp Icon: Original Design\
Merli Icon: https://www.sg101.gov.sg/resources/archives/identity-merli-the-mascot/ \
Button Icon: https://www.flaticon.com/

**Refrences**
Heritage information sourced from: 
- National Heritage Board https://www.nhb.gov.sg/
- Roots.sg https://www.roots.gov.sg/

## AI Tools Used
**Tool**: Claude \
**Usage**: 
- Stamp collection system 
- Debugging assistance and error resolution 
- Knowledge Points UI 

**Tool**: Gemini \
**Usage**: 
- Scene Management 
- Quiz system logic implementation 

## Wireframes
<img width="960" height="1280" alt="image" src="https://github.com/user-attachments/assets/99296105-8182-44f6-b906-6450c5181749" />

## AR Markers
https://www.canva.com/design/DAG6a1J7jRo/GBNRBrWLJjPqCBfrU2kuDw/edit?utm_content=DAG6a1J7jRo&utm_campaign=designshare&utm_medium=link2&utm_source=sharebutton
