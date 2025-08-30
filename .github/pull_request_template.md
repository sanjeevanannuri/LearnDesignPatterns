name: Pull Request
description: Create a pull request to contribute to this project
title: "[PR] "
body:
  - type: markdown
    attributes:
      value: |
        Thank you for contributing to Design Patterns Practice! Please fill out this template to help us review your changes.

  - type: dropdown
    id: type
    attributes:
      label: Type of Change
      description: What type of change does this PR introduce?
      options:
        - Bug fix (non-breaking change which fixes an issue)
        - New feature (non-breaking change which adds functionality)
        - Breaking change (fix or feature that would cause existing functionality to not work as expected)
        - Documentation update
        - Code refactoring
        - Performance improvement
        - Other
    validations:
      required: true

  - type: textarea
    id: description
    attributes:
      label: Description
      description: Please describe your changes in detail
    validations:
      required: true

  - type: textarea
    id: motivation
    attributes:
      label: Motivation and Context
      description: Why is this change required? What problem does it solve?

  - type: textarea
    id: testing
    attributes:
      label: How Has This Been Tested?
      description: Please describe the tests that you ran to verify your changes
      placeholder: |
        - [ ] Unit tests
        - [ ] Integration tests
        - [ ] Manual testing
        - [ ] Existing tests pass

  - type: textarea
    id: screenshots
    attributes:
      label: Screenshots (if appropriate)
      description: Add screenshots to help explain your changes

  - type: checkboxes
    id: checklist
    attributes:
      label: Checklist
      description: Please check all applicable items
      options:
        - label: My code follows the code style of this project
        - label: My change requires a change to the documentation
        - label: I have updated the documentation accordingly
        - label: I have added tests to cover my changes
        - label: All new and existing tests passed
        - label: I have checked my code and corrected any misspellings

  - type: textarea
    id: additional-notes
    attributes:
      label: Additional Notes
      description: Any additional information that would be helpful for reviewers
