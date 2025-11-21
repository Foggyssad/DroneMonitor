# DroneMonitor – Remote ID Data Collector & Analyzer (C# .NET)

This project processes BLE-based OpenDroneID / Remote ID telemetry exported from Wireshark.  
The program collects messages from a CSV file, groups them into tracks per transmitter, and computes data-quality metrics such as message gaps, static altitude, and duplicate messages.  
It also demonstrates a wide range of C# language features required by the assignment.

---

## Features

### Data Collection & Processing
- Imports and parses Wireshark-exported CSV files.
- Groups messages by BLE MAC address into **DroneTracks**.
- Calculates per-track:
  - Total messages
  - Duration
  - Message rate
  - Duplicate message count
  - Altitude stability
  - Message timing gaps
- Prints a summary for each track.

### C# Requirements Implemented
- Custom interfaces (`IDroneMessage`, `IDroneTrack`, `IAlertable`)
- `IComparable<T>` for ordering tracks
- `IEquatable<T>` for message equality
- `IFormattable`
- Pattern matching with `switch` + `when`
- Range operator (`^1`, etc.)
- Operator overloading (`+` to merge tracks)
- Abstract classes
- Sealed classes
- Static constructor
- Deconstructor usage (`var (ts, mac, mc) = msg`)
- Delegates / lambda functions (sorting, parsing)
- Bitwise operations with enum flags
- Null-coalescing operators (`??`, `?.`)
- `params` keyword
- Multiple modules (`DroneMonitor.Core` + `DroneMonitor.App`)

---

## Remote ID / OpenDroneID Format (Short Theory Section)

Remote ID (OpenDroneID) is a standard for unmanned aircraft to broadcast identity and telemetry via BLE or Wi-Fi.  
In BLE, this is embedded into Extended Advertising frames as Manufacturer Specific Data.

The main message types present in the capture:

| Type | Meaning |
|------|---------|
| **Basic ID (0)** | UAS serial/registration ID (hex string in this dataset). |
| **Location/Vector (1)** | Position, velocity, pressure altitude. |
| **System Message (4)** | System properties, status flags. |
| **Operator ID (5)** | Operator registration identifier. |

### CSV Columns Used
| Column | Meaning |
|--------|---------|
| Arrival Time | UTC timestamp when Wireshark captured the frame |
| Source | BLE MAC address (used as track identifier) |
| ID | Basic ID field (hex UAS identifier) |
| UA Pressure Altitude | Pressure altitude in feet |
| Message Type | OpenDroneID message type |
| Message Counter | Sequence counter |

Note: BLE privacy causes a single transmitter to rotate MAC addresses, so one physical drone may produce multiple “tracks.”

---

## Building and Running the Program

From the repo root:

```bash
dotnet build
cd DroneMonitor.App && dotnet run -- "test_DroneID_data.csv"