﻿# Backtrace C# Release Notes

## Version 1.1.1 - 06.04.2018
- Fix: Debug attributes now include namespaces
- Fix: Reporting unhandled application exceptions now uses SendAsync and proper TLS 1.2 support
- Fix: BacktraceClient.OnClientReportLimitReached event handlers will now take BacktraceReport as a parameter
- Refactoring of JSON Data code.

## Version 1.1.0 - 30.03.2018
- BacktraceClient now supports an asynchronously `SendAsync` method that works with `async task`
- For .NET Framework 4.5 and .NET Standard 2.0, `BacktraceClient` now streams file attachment content directly from disk via `SendAsync` method.
- `AfterSend` event parameter changed. Now `AfterSend` event require `BacktraceResult` parameter, not `BacktraceReport`,
- `Send` and `SendAsync` method now returns `BacktraceResult` with information about report state,
- `OnServerResponse` now require `BacktraceResult` as a parameter. 

## Version 1.0.0 - 19.03.2018
- First release.