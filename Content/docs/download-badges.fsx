(**
---
title: Adding download badges
category: sample content
categoryindex: 1
index: 5
---
*)

(**
# Adding download badges

## Script download

```no-highlight
[![Script]({{root}}img/badge-script.svg)]({{root}}{{fsdocs-source-basename}}.fsx)`
```

becomes

[![Script]({{root}}img/badge-script.svg)]({{root}}{{fsdocs-source-basename}}.fsx)

(you might need to adjust the paths if there are any more levels between `{{root}}` and `img/badge-script.svg` or `{{fsdocs-source-basename}}`)

## Notebook download

```no-highlight
[![Notebook]({{root}}img/badge-notebook.svg)]({{root}}{{fsdocs-source-basename}}.ipynb)
```

becomes

[![Notebook]({{root}}img/badge-notebook.svg)]({{root}}{{fsdocs-source-basename}}.ipynb)

(you might need to adjust the paths if there are any more levels between `{{root}}` and `img/badge-script.svg` or `{{fsdocs-source-basename}}`)

## Multiple badges in one line

To add multiple badges to appear on the same line like this:

[![Script]({{root}}img/badge-script.svg)]({{root}}{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook]({{root}}img/badge-notebook.svg)]({{root}}{{fsdocs-source-basename}}.ipynb)

add a `&emsp;` after the first two badges in you markdown:

```no-highlight
[![Script]({{root}}img/badge-script.svg)]({{root}}{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook]({{root}}img/badge-notebook.svg)]({{root}}{{fsdocs-source-basename}}.ipynb)
```
*)