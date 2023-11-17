(**
# Adding download badges

## Script download

```no-highlight
[![Script](https://fslab.org/docs-template/img/badge-script.svg)](https://fslab.org/docs-template/download-badges.fsx)`
```

becomes

[![Script](https://fslab.org/docs-template/img/badge-script.svg)](https://fslab.org/docs-template/download-badges.fsx)

(you might need to adjust the paths if there are any more levels between `{{root}}` and `img/badge-script.svg` or `{{fsdocs-source-basename}}`)

## Notebook download

```no-highlight
[![Notebook](https://fslab.org/docs-template/img/badge-notebook.svg)](https://fslab.org/docs-template/download-badges.ipynb)
```

becomes

[![Notebook](https://fslab.org/docs-template/img/badge-notebook.svg)](https://fslab.org/docs-template/download-badges.ipynb)

(you might need to adjust the paths if there are any more levels between `{{root}}` and `img/badge-script.svg` or `{{fsdocs-source-basename}}`)

## Multiple badges in one line

To add multiple badges to appear on the same line like this:

[![Script](https://fslab.org/docs-template/img/badge-script.svg)](https://fslab.org/docs-template/download-badges.fsx)&emsp;
[![Notebook](https://fslab.org/docs-template/img/badge-notebook.svg)](https://fslab.org/docs-template/download-badges.ipynb)

add a `&emsp;` after the first two badges in you markdown:

```no-highlight
[![Script](https://fslab.org/docs-template/img/badge-script.svg)](https://fslab.org/docs-template/download-badges.fsx)&emsp;
[![Notebook](https://fslab.org/docs-template/img/badge-notebook.svg)](https://fslab.org/docs-template/download-badges.ipynb)
```

*)

