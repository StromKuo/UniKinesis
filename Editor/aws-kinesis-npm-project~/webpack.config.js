const path = require('path');

module.exports = {
    mode: 'production',
    entry: './index.ts', // Entry file
    module: {
        rules: [
            {
                test: /\.tsx?$/, // Match .ts or .tsx files
                use: 'ts-loader',
                exclude: /node_modules/,
            },
        ],
    },
    resolve: {
        extensions: ['.tsx', '.ts', '.js'], // Automatically resolve files with these extensions
    },
    output: {
        filename: 'aws-kinesis-sdk.js',
        path: path.resolve(__dirname, 'dist'),
    },
};
