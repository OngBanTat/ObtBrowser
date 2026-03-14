from setuptools import find_packages, setup

setup(
    name="obt-browser-client",
    version="1.0.0",
    description="Python SDK for ObtAntiDetect Browser REST API",
    packages=find_packages(),
    python_requires=">=3.8",
    # No external dependencies — uses stdlib urllib only
)
