# Setup and Run

## 📦 Getting the latest release

There are three ways you can download GearstashBot:

 * Downloading [a release from Github](https://github.com/electricduck/gearstashbot/releases)
    * Supported platforms are:
        * Windows (7 or above), on x86_64 (`win-x64`) or ARM64 (`win-arm64`)
        * Windows Server (2008 R2 or above), on on x86_64 (`win-x64`) or ARM64 (`win-arm64`)
        * Linux, on x86_64 (`linux-x64`) or ARM64 (`linux-arm64`)
            * Distros using [musl](https://wiki.musl-libc.org/) _(a libc replacement)_, such as Alpine Linux, must use `linux.musl-x64`
        * macOS (10.12 Sierra or above), on x86_64 (`macos-x64`)
          * Currently, macOS builds have **not been tested**, but there is no reason that these shouldn't work as intended
 * Building it yourself, either manually, or using the the `BuildReleases.ps1` tool found in `./tools/`
    * Platforms that do not have downloads but are [listed as being supported by .NET Core](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog) (such as x86/32-bit or ARMv7 builds, or older macOS versions) must be built by you
 * Running it via Docker, using the provided `Dockerfile` found in `./src/GearstashBot/`

The first two methods will produce a portable singe-file binary that can be placed anywhere. It will create its needed files on first run; more details below. GearstashBot **can't currently update itself** or warn of newer versions: this is something that will be implemented at a later date.

The latest release of GearstashBot is **20.0.0-beta5**, released on 12th June 2020.

## 🤖 Setting up the bot

As with any Telegram bot, a bot token must first be made. To do this, open a chat with [@BotFather](https://t.me/botfather). Spark up a conversation with him by hitting **Start**: remember to be polite! Then:

 * Create a new bot with `/newbot`
 * Type the name you wish to give it; you can change this later.
 * Type the username you wish to give it, adhering to Telegram's bot username restrictions. Choose carefully, this cannot be changed!
 * Once completed, you should get your bot token: this is the long bunch of characters in the second paragraph, which will look something like `1234567890:AbC_dEfGhIjKlMnOpQrStUvWxYz`
 * You can change anything else you need about your bot using `/mybots` and selecting the bot's username. You can also see your bot token, again if you lost the original message.

You'll also need a channel for this bot to post to. If you haven't already created one, find **New Channel** in your Telegram app and go through the neccesary steps to create one.

Depending on the app you're using, adding the bot as an administrator is different. But in simple terms, it requires going to the **Channel Info** dialog (usually accessed by hitting the channel's name at the top of the UI), and then finding the **Administrators** option. When adding your bot, make sure it has the permission **Post messages**.

## 🔑 First run

### Prerequisites

 * [Python](https://www.python.org/) (3.x)
   * This is required for the necessary `gallery-dl` and `youtube-dl` to install/update.
   * Depending on your OS and method of install, [pip may need to be installed seperately](https://pip.pypa.io/en/stable/installing/).

### Running

To run, open the directory in a terminal and call the binary at the prompt, like

```
$ ./GearstashBot-x.x.x-xxx-xxx.bin
```

_See **Command line arguments** for a list of extra options_

The first run will fail, asking you to edit the settings file its just created. This is found in the newly-created `./config/` directory. You **must** edit this file and provide the correct settings to proceed. As explanation of the parameters can be found below (under **Config parameters**).

If you're running via Docker, you'll find a volume has been created. The `./config/` folder will be found in there.

Once configured, subsequent runs will connect to Telegram; along with updating the required packages from PIP, and making a backup of the database on startup. You can verify the bot is running properly by messaging the bot with `/info`.

#### Config parameters

 * `apiKeys`
   * `telegram` — The API key of your Telegram bot created with BotFather
 * `config`
   * `channel` — The ID of the channel you're posting to
     * There are a few ways to get the channel ID:
       * Using Telegram Web or Telegram CLI and following the methods here: https://github.com/GabrielRF/telegram-id
       * Using [Kotagram](https://kotatogram.github.io/) and getting the ID into the Channel Info dialog
         * Make sure to set **Settings > Kotatogram Settings > Chat ID in profile** to **Bot API**
     * The channel ID should be a negative number!
     * You cannot use the channel's username
   * `name` _(optional)_ — The name of the bot
     * This is visible in `/info`
     * Its reccomended to set this to the same name as your bot
     * Omitting this defaults to **GearstashBot**
   * `owner` — Your, or whoever owns the bots', username on Telegram
     * This is visible in `/info`, giving users someone to contact for help
     * This is also visible whenever an errors occurs, recommending the user forwards it on to you
   * `poll` — Whether to post things to the channel
     * Setting this to `false` is only useful for testing
   * `postInterval` — How often to post to the channel, in **milliseconds**
     * Some possible values:
       * `300000` (5 minutes)
       * `900000` (15 minutes)
       * `1800000` (30 minutes)
       * `3600000` (1 hour)

#### Command line arguments

There are some extra arguments that can be passed, all of which are optional and have default values

 * `-confdir` / `-c` — The directory whether the config (and database) is stored
   * Omitting this defaults to `./config/`

## 🤔 Using the bot

### The first user

Press **Start** (or type `/start`) to get the party going. This will create the first "author" (you) and grant you the permission to manage other users. It is recommended you type `/user` to set extra permissions (which is likely all of them), as these are not set by default.

#### Permissions

_(Todo)_

#### More users?

You know what they say, the more the merrior! There are two ways to add more members:

 * Ask them to start your bot. You'll get a notification about this, and can set their permissions with `/user <userID>` or `/user <username>`*
 * If you already know someone's ID beforehand, use `/user <userID>` to create the permissions for the author beforehand. This will "link up" when the user starts the bot.

_* Telegram's Bot API does not allow bots to grab user details automatically. The username, and the rest of their profile, will only be updated whenever the user interacts with the bot (or forcefully by using **🔄 Refresh Profile** in `/tools`). You can see when the profile was last updated by doing `/user <userID>` or `/user <username>`._

_(Todo)_

## 💥 Problems?

Please submit any problems to the [issue tracker on Github](https://github.com/electricduck/gearstashbot/issues), or [contact @theducky on Telegram](https://t.me/theducky).