# File Filter 🤖

AI-powered file content filtering tool using LLamaSharp and DeepSeek AI model.

## Features

- 🔍 **Smart Filtering**: Use natural language to describe what you're looking for
- 🤖 **AI-Powered**: Leverages DeepSeek-R1 model for intelligent text classification
- 📁 **File Output**: Save filtered results to a file
- 🖥️ **Cross-Platform**: Available for Windows, Linux, and macOS
- 📦 **Self-Contained**: No dependencies required

## Quick Start

### Installation

1. Download the latest release for your platform from the [Releases page](../../releases)
2. Extract the archive to your preferred location
3. Run from command line

### Basic Usage

```bash
# Filter lines containing animals and save to file
file-filter.exe data.txt -f "Find all entries that include animals" -o animals.txt

# Filter lines mentioning colors (output to console)
file-filter.exe log.txt -f "Find entries mentioning colors"

# Process file without specific filter (shows all processing)
file-filter.exe sample.txt
```

## Command Line Options

```
file-filter <input-file> [-f "filter prompt"] [-o "output-file"]
```

- `<input-file>`: Path to the file you want to filter (required)
- `-f "filter prompt"`: Natural language description of what to find (optional)
- `-o "output-file"`: Path where filtered results should be saved (optional)

## Examples

### Filter Log Files
```bash
# Find error entries in a log file
file-filter.exe app.log -f "Find entries that indicate errors or failures" -o errors.txt

# Find entries with specific IP addresses
file-filter.exe access.log -f "Find entries with suspicious IP addresses"
```

### Process Data Files
```bash
# Find entries about specific topics
file-filter.exe data.csv -f "Find rows containing product information" -o products.csv

# Filter by sentiment
file-filter.exe reviews.txt -f "Find positive reviews" -o positive-reviews.txt
```

### Content Analysis
```bash
# Find mentions of technologies
file-filter.exe documents.txt -f "Find lines mentioning programming languages or frameworks"

# Extract specific information
file-filter.exe emails.txt -f "Find emails containing meeting invitations" -o meetings.txt
```

## How It Works

File Filter uses the DeepSeek-R1 AI model to understand your natural language filter prompt and evaluate each line of your input file. The AI returns a simple true/false decision for each line, and matching lines are either displayed on screen or saved to your output file.

### AI Model Details

- **Model**: DeepSeek-R1-0528-Qwen3-8B (4-bit quantized)
- **Size**: ~4.6GB (included in release)
- **Language**: Optimized for English text
- **Performance**: Processes ~1-10 lines per second (depending on hardware)

## System Requirements

### Windows
- Windows 10 or later (x64)
- 8GB+ RAM recommended
- ~5GB free disk space

### Linux
- Ubuntu 18.04+ or equivalent
- x64 architecture
- 8GB+ RAM recommended
- ~5GB free disk space

### macOS
- macOS 10.15+ (Catalina)
- Intel x64 or Apple Silicon
- 8GB+ RAM recommended
- ~5GB free disk space

## Performance Tips

- **First Run**: Initial startup may take 30-60 seconds as the AI model loads
- **Large Files**: For files with thousands of lines, consider processing in chunks
- **Memory**: Ensure adequate RAM for both the model and your file size
- **Storage**: SSD storage will improve model loading times

## Building from Source

### Prerequisites
- .NET 8.0 SDK or later
- Git

### Build Steps
```bash
git clone <repository-url>
cd file-filter
dotnet restore
dotnet build -c Release
```

### Creating Releases
```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# Linux
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true

# macOS
dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true
```

## Troubleshooting

### Common Issues

**"Model file not found"**
- Ensure the `models` folder is in the same directory as the executable
- Check that `DeepSeek-R1-0528-Qwen3-8B-IQ4_XS.gguf` is present

**Slow performance**
- First run is always slower due to model initialization
- Ensure adequate RAM (8GB+)
- Consider using SSD storage
- Close other memory-intensive applications

**"Unclear response" warnings**
- The AI model occasionally returns ambiguous results
- These lines are marked as non-matches by default
- Consider rephrasing your filter prompt for better results

**File access errors**
- Ensure you have read permissions for input files
- Ensure you have write permissions for output directory
- Check that files aren't locked by other applications

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- [LLamaSharp](https://github.com/SciSharp/LLamaSharp) - .NET bindings for LLaMA
- [DeepSeek](https://www.deepseek.com/) - AI model provider
- [Microsoft .NET](https://dotnet.microsoft.com/) - Runtime and SDK
