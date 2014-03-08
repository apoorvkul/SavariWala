namespace java com.savariwala
namespace csharp SavariWala
namespace cpp savariwala

enum ErrorCode {
  Unspecified = 0
  NotFound = 1
}

exception ServerError {
  1: string what,
  2: ErrorCode err
}

