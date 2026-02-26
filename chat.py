"""Console chat client for the Simple Agent function app."""
import os
import urllib.request

BASE_URL = os.environ.get("AGENT_URL", "http://localhost:7071")

print(f"=== Simple Agent Chat ===")
print(f"Endpoint: {BASE_URL}/api/ask")
print(f"Type 'exit' or 'quit' to end.\n")

while True:
    message = input("You: ").strip()
    if not message or message.lower() in ("exit", "quit"):
        print("Goodbye!")
        break

    url = f"{BASE_URL}/api/ask"
    try:
        req = urllib.request.Request(url, data=message.encode(), method="POST")
        with urllib.request.urlopen(req) as resp:
            print(f"\nAgent: {resp.read().decode()}\n")
    except Exception as e:
        print(f"\nError: {e}\n")
