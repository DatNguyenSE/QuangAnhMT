const fs = require('fs');
const path = require('path');

function processFile(filePath) {
    let content = fs.readFileSync(filePath, 'utf8');
    let originalContent = content;

    // 1. Replace simple String Interpolation
    // This is a naive replacement that works for most simple cases in this codebase.
    // It finds $"" and $@""
    let regexInterpolation = /\$@?"([^"\\]*(?:\\.[^"\\]*)*)"/g;
    content = content.replace(regexInterpolation, (match, inner) => {
        let isVerbatim = match.startsWith('$@');
        let args = [];
        let formatStr = inner.replace(/\{([^}]+)\}/g, (m, expr) => {
            // handle formatting like {pct:0.#}
            let colonIdx = expr.indexOf(':');
            let arg = expr;
            let format = '';
            if (colonIdx !== -1) {
                arg = expr.substring(0, colonIdx);
                format = expr.substring(colonIdx);
            }
            args.push(arg);
            return `{${args.length - 1}${format}}`;
        });
        
        if (args.length === 0) {
            return (isVerbatim ? '@"' : '"') + formatStr + '"';
        }
        
        return `string.Format(${isVerbatim ? '@"' : '"'}${formatStr}", ${args.join(', ')})`;
    });
    
    // Also handle multiline $@""
    let regexMultiInterpolation = /\$@\"([\s\S]*?)\"/g;
    content = content.replace(regexMultiInterpolation, (match, inner) => {
        let args = [];
        let formatStr = inner.replace(/\{([^}]+)\}/g, (m, expr) => {
            let colonIdx = expr.indexOf(':');
            let arg = expr;
            let format = '';
            if (colonIdx !== -1 && !expr.includes('?')) {
                // heuristic to avoid replacing inside ternary ops
                arg = expr.substring(0, colonIdx);
                format = expr.substring(colonIdx);
            }
            args.push(arg);
            return `{${args.length - 1}${format}}`;
        });
        if (args.length === 0) {
            return `@"${formatStr}"`;
        }
        return `string.Format(@"${formatStr}", ${args.join(', ')})`;
    });

    // 2. Replace Null-conditional operator ?.
    // E.g., ViewState["taikhoan"]?.ToString() -> (ViewState["taikhoan"] != null ? ViewState["taikhoan"].ToString() : null)
    // E.g., stats?.Count -> (stats != null ? stats.Count : null)
    // We will do this manually for the matched patterns since they are very specific in the grep output.
    
    // 3. Fix expression bodied methods/properties
    // This requires regex to find public Type Property => expr;
    let propRegex = /\b(public|private|protected|internal|static)\s+([\w<>\[\]]+)\s+(\w+)\s*=>\s*([^;]+);/g;
    content = content.replace(propRegex, "$1 $2 $3 { get { return $4; } }");

    let methodRegex = /\b(public|private|protected|internal|static)\s+([\w<>\[\]]+)\s+(\w+)\s*\(([^)]*)\)\s*=>\s*([^;]+);/g;
    content = content.replace(methodRegex, "$1 $2 $3($4) { return $5; }");
    
    if (content !== originalContent) {
        fs.writeFileSync(filePath, content, 'utf8');
        console.log(`Updated ${filePath}`);
    }
}

function walkSync(dir) {
    let files = fs.readdirSync(dir);
    files.forEach(file => {
        let filepath = path.join(dir, file);
        let stat = fs.statSync(filepath);
        if (stat.isDirectory()) {
            walkSync(filepath);
        } else if (file.endsWith('.cs')) {
            processFile(filepath);
        }
    });
}

walkSync(__dirname);
console.log('Done');
