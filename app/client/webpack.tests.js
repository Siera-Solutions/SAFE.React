// Template for webpack.config.js in Fable projects
// In most cases, you'll only need to edit the CONFIG object (after dependencies)
// See below if you need better fine-tuning of Webpack options

// Dependencies. Also required: core-js, @babel/core,
// @babel/preset-env, babel-loader
var path = require("path");
var webpack = require("webpack");
var HtmlWebpackPlugin = require('html-webpack-plugin');
var CopyWebpackPlugin = require('copy-webpack-plugin');
var MiniCssExtractPlugin = require("mini-css-extract-plugin");
const Dotenv = require('dotenv-webpack');
const { patchGracefulFileSystem } = require("./webpack.common.js");
patchGracefulFileSystem();

var CONFIG = {
    // The tags to include the generated JS and CSS will be automatically injected in the HTML template
    // See https://github.com/jantimon/html-webpack-plugin
    indexHtmlTemplate: "./tests/index.html",
    fsharpEntry: "./tests/Tests.fs.js",
    outputDir: "./dist",
    assetsDir: "public",
    devServerPort: 8085,
    // When using webpack-dev-server, you may need to redirect some calls
    // to a external API server. See https://webpack.js.org/configuration/dev-server/#devserver-proxy
    devServerProxy: undefined,
}

// If we're running the webpack-dev-server, assume we're in development mode
var isProduction = !process.argv.find(v => v.indexOf('webpack-dev-server') !== -1);
console.log("Bundling for " + (isProduction ? "production" : "development") + "...");

// The HtmlWebpackPlugin allows us to use a template for the index.html page
// and automatically injects <script> or <link> tags for generated bundles.
var commonPlugins = [
    new HtmlWebpackPlugin({
        filename: 'index.html',
        template: resolve(CONFIG.indexHtmlTemplate)
    }),

    new Dotenv({
        path: "./.env",
        silent: false,
        systemvars: true
    })
];

module.exports = {
    // In development, bundle styles together with the code so they can also
    // trigger hot reloads. In production, put them in a separate CSS file.
    entry: {
        app: [resolve(CONFIG.fsharpEntry)]
    },
    // Add a hash to the output file name in production
    // to prevent browser caching if code changes
    output: {
        path: resolve(CONFIG.outputDir),
        filename: isProduction ? '[name].[hash].js' : '[name].js'
    },
    mode: isProduction ? "production" : "development",
    devtool: isProduction ? "source-map" : "eval-source-map",
    optimization: {
        // Split the code coming from npm packages into a different file.
        // 3rd party dependencies change less often, let the browser cache them.
        splitChunks: {
            chunks: "all"
        },
    },
    plugins: commonPlugins.concat([
        new CopyWebpackPlugin({
            patterns: [
                { from: resolve(CONFIG.assetsDir) }
            ]
        }),
    ]),
    // Configuration for webpack-dev-server
    devServer: {
        static: {
            publicPath: "/",
            directory: resolve(CONFIG.assetsDir)
        },
        port: CONFIG.devServerPort,
        proxy: CONFIG.devServerProxy,
        hot: true
    },
    // - sass-loaders: transforms SASS/SCSS into JS
    module: {
        rules: [
            {
                test: /\.(js)$/,
                exclude: /node_modules/,
                enforce: "pre",
                use: ["source-map-loader"]
            },
            {
                test: /\.(sass|scss|css)$/,
                use: [
                    isProduction
                        ? MiniCssExtractPlugin.loader
                        : 'style-loader',
                    {
                        loader: 'css-loader'
                    },
                    {
                        loader: 'sass-loader',
                        options: { implementation: require("sass") }
                    }
                ],
            },
            {
                test: /\.(png|jpg|jpeg|gif|svg|woff|woff2|ttf|eot)(\?.*)?$/,
                type: 'assets/resource'
            }
        ]
    }
};

function resolve(filePath) {
    return path.isAbsolute(filePath) ? filePath : path.join(__dirname, filePath);
}
