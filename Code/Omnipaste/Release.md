**Version 1.5.3 (31.03.2015)**

**Bug fixes:**

- Fixes msvcr110 include on windows 7.

----------

**Version 1.5.3 (31.03.2015)**

**Bug fixes:**

- Fixes contact update for some users

**Improvements:**

- Better duplicate contact handling

----------

**Version 1.5.0 (27.03.2015)**

**Features:**

- Adds ability to send group messages.

----------

**Version 1.4.3 (26.03.2015)**

**Features:**

- Data is stored locally, no more tabula rasa after application restart.

----------

**Version 1.3.17 (11.03.2015)**

**Features:**

- Contact images are now displayed.
- Improved caller id functionality

----------

**Version 1.3.16 (11.03.2015)**

**Features:**

- You can now access all your contacts from the People screen


----------

**Version 1.3.15 (09.03.2015)**

**Bug fixes:**

- Fixed conversations local conversations appearing as from remote

**Improvements:**

- Removes all old code related to click-once install


----------

**Version 1.3.14 (24.02.2015)**

**Bug fixes:**

- Fixed false threat warning of Norton Internet Security

----------

**Version 1.3.13 (21.02.2015)**

**Bug fixes:**

- fixed interaction between installer and app while updating that caused app to crash
- fixed registration issue that prevented user from using the app

----------

**Version 1.3.12 (19.02.2015)**

**Improvements:**

- added global exception logging

----------

**Version 1.3.11 (19.02.2015)**

**Features:**

- implemented new Clipping screen

----------

**Version 1.3.10 (19.02.2015)**

**Bug fixes:**

- fixed issue with time for outgoing messages that was set to a value prior to the time the message was sent
- fixed issue with release log text not being left aligned

----------

**Version 1.3.9 (12.02.2015)**

**Improvements:**

- added better exception handling

----------

**Version 1.3.8 (11.02.2015)**

**Improvements:**

- added some minor usability improvements to the way switching between workspaces works

----------

**Version 1.3.7 (09.02.2015)**

**Features:**

- added links to social networks on the release log screen so that users can comment on the release
- contacts are now ordered by the last time an interaction with them was detected

----------

**Improvements:**

- updated the app so as to use the system provided window animations when minimizing/restoring the main window
- added better handling for cases when a device is no longer trusted/ the user logged of
- current contact/activity selection is saved when switching between different screens

----------

**Bug fixes:**

- fixed a bug which would sometimes crash the app if the user would try to select some text in the release notes activity

----------


**Version 1.3.6 (02.02.2015)**

**Improvements:**

- added better phone number matching for grouping calls/messages by contact

----------

**Version 1.3.5 (02.02.2015)**

**Features:**

- added Messages screen where conversations are grouped by contact
- added ability to filter contacts by name/phone number
- added ability to add contact to favorites
- added ability to filter favorite contacts
- displayed last message text / last call details on contact

----------

**Version 1.3.4 (29.01.2015)**

**Bug fixes:**

- fixed notifications not being received after switching to other network connection

----------

**Version 1.3.3 (28.01.2015)**

**Features:**

- scroll on top of activity list when activity is created

**Improvements:**

- displayed hand cursor only when hovering an activity
- added active/inactive states for activity search box

**Bug fixes:**

- fixed color code for delete header
- fixed color code for call initiated/calling header

----------

**Version 1.3.2 (21.01.2015)**

**Bug fixes:**

- fixed spelling mistakes

----------

**Version 1.3.1 (20.01.2015)**

**Bug fixes:**

- fixed shortcut keys not working after switching between screens

----------

**Version 1.3 (19.01.2015)**

**Bug fixes:**

- fixed issue where a "New Version Available" activity would appear multiple times

**Improvements:**

- migrated to using the new, more powerful and flexible Omnipaste API
- updated the style for the activity type filter to make it more obvious that it is a filter
- modified the auto-update process so as to restore the application window to its previous state (minimized/maximized) after installing an update


**New features**

- added outgoing calls to the conversation screen

----------

**Version 1.2.2 (15.01.2015)**

**Bug fixes:**

 - fixes issue with app not working with certain system proxy configurations

**Improvements:**

- added some nice transitions for activity item added, deleted/restored and also for the notifications being dismissed
- other minor improvements

----------

**Version 1.2.1 (14.01.2015)**

**Improvements:**

- added some nice transitions for opening an activity details screen and for switching between different activity details headers

----------

**Version 1.2 (12.01.2015)**

**Bug fixes:**

 - images in the conversation screen did not appear in a rounded circle/had some artefacts
 - in the conversation screen, users were unable to type in the reply text box if it became empty at a certain point

**Improvements:**

 - improved the conversation screen: 

	- the reply text box is focused when opening the screen

	- the reply text box is automatically focused after a message is sent 

	- the latest item in the conversation is automatically scrolled into view

	- allow users to press the 'Enter' key to send a reply and use 'SHIFT+Enter' to add new line

 - update the layout of the settings button on the side bar so as to make it more visible
 - improved the design of the scroll bars/scroll viewers in the application
 - added window chrome to make the application look more like a native application
 - trimmed white space characters from clippings in the activity list screen so as to avoid having an empty looking activity
 - limited the maximum size for an unread activity in the activity list to 10 lines

**New features**

 - Hide notifications for activity items that have been viewed
 - Add actions for activity details

	- allow users to call back a contact from a conversation screen

	- allow users to delete clippings/conversations

	- allow users to copy an old clipping

	- detect hyperlinks in clipping content and allow users to press/follow them

 - show new version info to users using an activity entry
 - allow users to filter items on the activity list screen based on their text content

**Miscellaneous**

 - Removed the holidays module as it doesn't make sense any more