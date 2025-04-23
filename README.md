# MoreRealisticLaundering
![LaunderingIcon](https://github.com/user-attachments/assets/5c807458-11ea-47bb-a13a-af9b8e534587)
## Summary

MoreRealisticLaundering enhances the in-game money laundering mechanics by introducing more detailed customization and management options for businesses.

## Installation
1. Download the latest release from the [GitHub Releases](https://github.com/user/MoreRealisticLaundering/releases) page.
2. Extract the `Mods` and `UserData` folders of the ZIP file into your game's directory.
3. Launch the game, and the mod will automatically initialize.

---

## Configuration
- The mod generates a configuration file (`MoreRealisticLaundering.json`) in the `UserData` folder.
- You can edit this file to customize the default settings for each business however this is optional
- Use the custom app ingame to modify everything dynamically

---

## Features

### Dynamic App Integration:
- Adds a custom app to the in-game phone, enabling players to manage their businesses directly from the app.
- The app includes detailed options for each business, such as adjusting settings and viewing business-specific information.

![App Icon](https://github.com/user-attachments/assets/46d504fb-d764-4bb3-bf3a-a7760cb7376f)

### Automatic Business Detection:
- Automatically detects newly owned businesses and adds them to the app for management.
- Ensures that all businesses are processed and updated dynamically.

### Customizable Business Settings:
- Allows players to adjust the maximum laundering capacity, laundering time, and taxation percentage for each business (e.g. Laundromat, Taco Ticklers, Car Wash, Post Office).
- These settings can be saved and applied dynamically through an intuitive interface.

![App Content New](https://github.com/user-attachments/assets/8ef1ddc0-5485-406b-a69c-9ad49d665718)

### Realistic Taxation System:
- Implements a taxation system that deducts a percentage of laundered money based on the configured tax rate for each business.
- Displays notifications for tax deductions and provides detailed logs of transactions.

### Improved Laundering Mechanics:
- Adjusts laundering times and capacities dynamically based on the player's configuration.
- Adjusts laundering operations to reflect the configured settings, ensuring a more realistic and personalized experience.

![Correct Launder Computers](https://github.com/user-attachments/assets/415b77d6-353c-4215-b080-8b74ba7a50d2)

### Custom Notifications:
- Sends in-game notifications for key events, such as when changes are saved, taxes are deducted, or laundering operations are completed.

![Tax Notification](https://github.com/user-attachments/assets/11ae48d4-dc88-482e-83e8-1ed492a0ec3a)

---

## New Features and Changes

### Enhanced Real Estate Management:
- Introduces a dedicated "Ray's Real Estate" section in the app for managing property prices (currently only businesses).
- Allows players to view and adjust the prices of unowned (and owned) businesses directly from the app.
  
  ![Rays Real Estate](https://github.com/user-attachments/assets/869aa50b-d60a-49f2-99cf-2936fd200075)

### Property Listings Price Updates:
- Automatically updates the prices displayed on the property listings in "Ray's Real Estate Office" based on the player's configuration.
- Ensures that the displayed prices are always accurate and reflect the latest settings.
  
![Properties Whiteboard](https://github.com/user-attachments/assets/96145491-afef-4aa5-abb4-a9b506e21d9c)

### Sell Sign Price Updates:
- Updates the prices displayed on the "For Sale" signs for each business (e.g., Laundromat, Taco Ticklers, Car Wash, Post Office).
- Ensures that the prices on the signs match the configured values.
  
![Property Signs](https://github.com/user-attachments/assets/a9bda137-ad6e-4bdb-81d0-18631dd660f9)

### Improved Input Validation:
- Ensures that all input fields in the app are validated to prevent invalid values (e.g. prices below a minimum threshold of $1000).

### Dynamic Price Application:
- Dynamically applies updated prices to all relevant in-game objects, including property listings and sell signs, ensuring consistency across the game world.

![Correct Dialogs](https://github.com/user-attachments/assets/6308dc12-2db2-480c-948c-03127f846d8b)

### Switched to NET472:
- The mod now targets .NET Framework 4.7.2 for improved compatibility with Unity and MelonLoader.
- Ensures better support for legacy systems and avoids runtime issues caused by incompatible .NET versions.
- Simplifies deployment by removing the need for users to install additional .NET runtimes.

---

## Download Mirrors
- **[NexusMods](https://www.nexusmods.com/schedule1/mods/775?tab=description)**
- **[ThunderStore](https://thunderstore.io/c/schedule-i/p/KampfBallerina/MoreRealisticLaundering/)**
