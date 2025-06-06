using System.ComponentModel;

namespace RPC;

public class CallMetadata : INotifyPropertyChanged
{
    private int? _responseLength;
    private int? _responseBytesRead;
    private double? _responseBandwidth;

    public int? ResponseLength
    {
        get => _responseLength;
        set
        {
            if (_responseLength == value) return;
            _responseLength = value;
            OnPropertyChanged(nameof(ResponseLength));
        }
    }

    public int? ResponseBytesRead
    {
        get => _responseBytesRead;
        set
        {
            if (_responseBytesRead == value) return;
            _responseBytesRead = value;
            OnPropertyChanged(nameof(ResponseBytesRead));
        }
    }

    public double? ResponseBandwidth
    {
        get => _responseBandwidth;
        set
        {
            _responseBandwidth = value;
            OnPropertyChanged(nameof(ResponseBandwidth));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
