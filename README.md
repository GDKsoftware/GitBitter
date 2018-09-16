# GitBitter

[![Build Status](https://api.travis-ci.org/GDKsoftware/GitBitter.svg?branch=master)](https://travis-ci.org/GDKsoftware/GitBitter)

## Tools

* GitBitterEdit - To edit a gitbitter.json and add BitBucket repositories.
* GitBitter - A commandline tool to clone or pull packaged repositories.


The GitBitterEdit tool is currently Windows-only, but the GitBitter command line can be used with Mono.

## Usage

Run GitBitterEdit on Windows if you can, you'll be able to log in to your Github or Bitbucket account and list all the projects you have access to.

Or create a gitbitter.json file in your projects folder, and run gitbitter.exe from this directory.

### Example gitbitter.json

```
{
  "Packages": [
    {
      "Folder": "Mime",
      "Repository": "https://github.com/GDKsoftware/Filename-extension-Mime.git",
      "Branch": "master"
    },
    {
      "Folder": "Spring",
      "Repository": "https://bitbucket.org/sglienke/spring4d.git",
      "Branch": "master"
    }
  ]
}
```

## Gitbitter configuration

If you're not using GitBitterEdit, you might want to know a way to configure some settings. We use an ini style file in your user directory \users\yourusername\.gitcredentials on Windows, and /home/yourusername/.gitcredentials on Linux/MacOSX.

### Example .gitcredentials

```
[gitbitter]
usessh=true
useresethard=true
```

The usessh flag changes the urls to clone to their ssh equivalents while cloning the repositories. This will be almost always have to be set to true when you don't use GitBitterEdit.

The useresethard flag will always do a 'git reset --hard' command before fetching the latest updates from the repository. If you don't accidentally want to lose changes you've done to the references repositories, you should set this to false. Be aware that you might get errors while running gitbitter if you do, this is to warn you there are uncommitted changes somewhere.
