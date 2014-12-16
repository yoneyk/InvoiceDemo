# &lt;gaplyk&gt;

> numeric input formatted to regional currency settings using Intl API

## Install

Install the component using [Bower](http://bower.io/):

```sh
$ bower install gaplyk/input-currency --save
```

Or [download as ZIP](https://github.com/gaplyk/input-currency/archive/master.zip).

## Usage

1. Import Web Components' polyfill:

    ```html
    <script src="bower_components/platform/platform.js"></script>
    ```

2. Import Custom Element:

    ```html
    <link rel="import" href="bower_components/gaplyk/src/input-currency.html">
    ```

3. Start using it!

    ```html
    <input-currency></input-currency>
    ```

## Options

Attribute     | Options     | Default      | Description
---           | ---         | ---          | ---
`locales`     | *string*    | `en-US`      | BCP 47 language tag. 
`currency`    | *string*    | `USD`        | The currency to use in currency formatting.
`value`       | *string*    | ``           | Entered value
`placeholder` | *string*    | ``           |
`readonly`    | *boolean*   | false        | Add grey background and still allow you to edit field. When content is changed background of input changed to another color.

## Events

Event           | Description
---             | ---
`value-changed` | Triggers when value changed.

## Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -m 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D

## License

[MIT License](http://opensource.org/licenses/MIT)
