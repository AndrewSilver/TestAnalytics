const path = require('path');

module.exports = {
    mode: 'development',
    entry: {"main": "./Client/src/index.tsx"},
    output: {
        filename: "bundle.js",
        path: path.resolve(__dirname, 'wwwroot/source/'),
        publicPath: 'source/'
    },

    devtool: "source-map",

    resolve: {
        extensions: [".ts", ".tsx", ".js", ".json"]
    },

    module: {
        rules: [
            { 
                test: /\.tsx?$/, 
                // exclude: /(node_modules)/,
                // use: {
                //     loader: 'babel-loader',
                //     options: {
                //         presets: ['@babel/preset-typescript','@babel/preset-react', '@babel/preset-env']
                //     }
                // }
                loader: "ts-loader",
                options: {
                    transpileOnly: true //HMR doesn't work without this
                }
            },
            { 
                enforce: "pre", 
                test: /\.js$/, 
                loader: "source-map-loader" 
            },
            {
                test: /\.css$/, 
                use: [
                   { loader: "style-loader" },
                   { loader: "css-loader" }
                ]
             }
        ]
    },
    externals: {
        'react': 'React',
        'react-dom': 'ReactDOM',
    },
};