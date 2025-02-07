const path = require('path');

module.exports = {
    entry: './src/index.js', // Path to your main JS file (e.g., index.js inside the 'src' folder)
    output: {
        filename: 'bundle.js', // The name of the output file after bundling
        path: path.resolve(__dirname, 'dist'), // Output folder
    },
    module: {
        rules: [
            {
                test: /\.js$/, // Apply Babel to .js files
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        presets: ['@babel/preset-env'], // Using Babel to convert modern JS to browser-compatible code
                    },
                },
            },
        ],
    },
    mode: 'development', // Change to 'production' for optimized output
};
