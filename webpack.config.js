const MonacoWebpackPlugin = require('monaco-editor-webpack-plugin');
const path = require("path");
const webpack = require("webpack");
const HtmlWebpackPlugin = require('html-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');

var babelOptions = {
    presets: [
        ["@babel/preset-env", {
            "targets": {
                "browsers": ["last 2 versions"]
            },
            "modules": false,
            "useBuiltIns": "usage",
            "corejs": 3,
            // This saves around 4KB in minified bundle (not gzipped)
            "loose": true,
        }]
    ],
};

var commonPlugins = [
    new HtmlWebpackPlugin({
        filename: './index.html',
        template: './src/Tempo.Demo/index.html'
    })
];

var monaco = {
    languages: ['html', 'fsharp']
}

module.exports = (env, options) => {

    // If no mode has been defined, default to `development`
    if (options.mode === undefined)
        options.mode = "development";

    var isProduction = options.mode === "production";
    console.log("Bundling for " + (isProduction ? "production" : "development") + "...");

    return {
        devtool: undefined,
        mode: options.mode,
        entry: {
            demo: [
                // "@babel/polyfill",
                './src/Tempo.Demo/App.fs.js',
                './src/Tempo.Demo/css/main.css'
            ]
        },
        output: {
            path: path.join(__dirname, './output'),
            filename: isProduction ? '[name].[chunkhash].js' : '[name].js'
        },
        plugins: isProduction ?
            commonPlugins.concat([
                new CopyWebpackPlugin({
                    patterns: [
                        { from: './static' }
                    ]
                }),
                new MonacoWebpackPlugin(monaco)
            ])
            : commonPlugins.concat([
                new MonacoWebpackPlugin(),
                new webpack.HotModuleReplacementPlugin(),
            ]),
        devServer: {
            contentBase: './static/',
            publicPath: "/",
            port: 8081,
            hot: true,
            inline: true,
            historyApiFallback: true
        },
        module: {
            rules: [
                {
                    test: /\.js$/,
                    exclude: /node_modules/,
                    use: {
                        loader: 'babel-loader',
                        options: babelOptions
                    },
                },
                {
                    test: /\.css$/,
                    use: ['style-loader', 'css-loader', 'postcss-loader']
                },
                {
                    test: /\.(png|jpg|jpeg|gif|svg|woff|woff2|ttf|eot)(\?.*$|$)/,
                    use: ["file-loader"]
                }
            ]
        }
    };
}
