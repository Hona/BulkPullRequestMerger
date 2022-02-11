# BulkPullRequestMerger
Approves &amp; merges the masses of PRs made by CMS's like Contentful

[**Download the latest release here**](https://github.com/Hona/BulkPullRequestMerger/releases)

# Usage

You must run the setup config command first to generate a GithubConfig.json file which the later steps use. It is created in the same directory as the exe.

## Setup

`.\BulkPullRequestApprover.exe config update --username "YourGitHubUsername" --pat "ghp_YourPATfromSETTINGS" --target-owner "UsernameOrOrganisation" --target-repo "repositoryName"`

## Mass Approve & Merge

`.\BulkPullRequestApprover.exe prs merge`

## Mass Close

`.\BulkPullRequestApprover.exe prs close`
