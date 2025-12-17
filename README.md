# DroneMonitor – Remote ID Data Collector and Analyzer (C# .NET)

This project processes OpenDroneID / Remote ID telemetry exported from Wireshark (with the open source OpenDroneID dissector plugin) obtained by the Nordic BLE sniffer. A separate DJI transmitter PCB was used for testing.

The program collects messages from a .csv file, groups them into tracks per MAC address, and computes metrics, such as: amount of messages, message rate, amount of duplicate messages, duration of overall transmition per the MAC address.  
It also demonstrates a wide range of C# language features required by the assignment.

---

## Features (Functional Requirements)

### Data Collection and Processing
- Imports and parses .csv files exported from the Wireshark.
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
- `IEquatable<T>` for message equality (used in the duplicate identification)
- `IFormattable`
- Pattern matching with `switch` + `when`
- Range operator (`^1`, etc.)
- Operator overloading (`+` to merge tracks)
- Abstract classes
- Sealed classes
- Static constructor
- Deconstructor usage
- Delegates / lambda functions (sorting, parsing)
- Bitwise operations with enum flags
- Null-coalescing operators (`??`, `?.`)
- `params` keyword
- Multiple modules (`DroneMonitor.Core` + `DroneMonitor.App`)

---

## Advanced C# Features (Second Assignment)

Additional functionality was implemented mainly in the `DroneMonitor.Analytics` module:

- Custom iteration using `IEnumerable<T>` and `IEnumerator<T>` over drone messages.
- Explicit iterator usage via `foreach` in the application logic.
- Generic analytics container with constraints (`MetricAccumulator<T>` using `where`).
- Custom exception hierarchy with centralized `try–catch` handling.
- Extension methods and an extension deconstructor for `DroneTrack` reporting.
- Event-based message observation (`TrackEventHub`).
- Safe message snapshotting via `ICloneable`.

These features are exercised during normal execution and reporting.

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

### .csv Columns Used (Please, manually select them in the Wireshark before exporting)
| Column | Meaning |
|--------|---------|
| Arrival Time | UTC timestamp when Wireshark captured the frame |
| Source | BLE MAC address (used as track identifier) |
| ID | Basic ID field (hex UAS identifier) |
| UA Pressure Altitude | Pressure altitude in feet |
| Message Type | OpenDroneID message type |
| Message Counter | Sequence counter |

Note: BLE encryption protocol causes the transmitter to rotate MAC addresses, so one physical transmitter may produce multiple tracks.

---

## Building the Program

From the repo root:

```bash
dotnet build
```
---

## Usage

### Process the existing .csv file (default mode)

From the repo root:

```bash
dotnet run --project DroneMonitor.App -- "DroneMonitor.App/test_DroneID_data.csv"
```

or if you prefer to run from inside the DroneMonitor.App directory:

```bash
cd DroneMonitor.App && dotnet run -- "test_DroneID_data.csv"
```

### Collect (Append) one new message via CLI to the existing .csv file

This starts interactive prompts and appends a new row to the .csv file:

```bash
dotnet run --project DroneMonitor.App -- "DroneMonitor.App/test_DroneID_data.csv" collect
```

You will be asked to enter:

- MAC (Source)

- Basic ID (optional)

- Pressure Altitude feet (optional)

- Message Type (Basic ID / Location/Vector / Operator ID / System)

- Message Counter (optional)
