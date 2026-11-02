# Contributing
This document contains the instructions and procedures required to contribute to the project. Listed below are the expectations and procedures our team has for branches, commits, pull requests, "definition of done", and more.

## Local Checks 
TBD 

## Development Setup
Use the Unity Editor Version listed in [README.md](./README.md). The Unity Editor tools and libraries for XR applications must be included. 

## Branches and Commits
`main` will contain all the up-to-date approved changes, and it is recommended to pull the repo from there unless you are adding to on-going work. For developing on ongoing work, the branches will be named in accordance with the module or issues that they are addressing. Changes from these branches may be merged into `main` during the development process. However, unapproved commits will remain in their original branch until a team member has approved them. Commit messages must be as detailed as possible, covering what work was done, and how it has addressed the requirements set out for that module. Any requirements not yet met must also be noted in the commit message.

## Pull Requests
All code pull requests (anything that is not exclusively docs-related) must be approved by a team member that is not the contributor. This rule is intended to ensure code quality, and prevent issues/conflicts with pre-existing code or the project requirements. Unless discussed and agreed upon otherwise by the team, all pull requests will only require one teammate's approval in order to be merged with `main` (assuming the branch passes all required tests). This teammate must be another developer assigned to the issue or module your commit is addressing, or a developer that has familiarized themselves with the issue when there is not another teammate assigned to your issue. 

## Definition of Done
An issue, module, or branch of the project will only be considered "done" when it has met or exceeded all requirements set out for it, passes all local tests (ensuring functionality across all possible cases), passes the CI tests, and does not conflict with or impair the quality and function of the pre-existing code.

## Where It Lives
For any files that are not code-related (ie. docs or assets), there exists designated folders for these and all files must be organized into these folders accordingly. Any files or file edits that will not impact code may be pushed to `main` without a pull request or approval from another team member. For example, changes to the README will not require a teammate to submit and await approval on a pull request. These changes may be pushed straight to `main` or the desired branch. 

## Getting Help
For any questions regarding contributions, please email Ethan (robinset@oregonstate.edu). A response can be expected within 1-2 days.
