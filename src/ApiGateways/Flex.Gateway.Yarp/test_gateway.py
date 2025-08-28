#!/usr/bin/env python3
"""
Test script for Flex.Gateway.Yarp functionality
This script tests the gateway's basic functionality without requiring protobuf files.
"""

import requests
import time
import json

class GatewayTester:
    def __init__(self, gateway_host="localhost", gateway_port=5000):
        self.gateway_host = gateway_host
        self.gateway_port = gateway_port
        self.base_url = f"http://{gateway_host}:{gateway_port}"
        
    def test_health_check(self):
        """Test the gateway health endpoint"""
        try:
            health_url = f"{self.base_url}/health"
            print(f"Testing health check at {health_url}")
            
            response = requests.get(health_url, timeout=5)
            print(f"Health check status: {response.status_code}")
            
            if response.status_code == 200:
                health_data = response.json()
                print(f"Health check response: {json.dumps(health_data, indent=2)}")
                return True
            else:
                print(f"Health check failed with status: {response.status_code}")
                return False
                
        except Exception as e:
            print(f"Health check failed: {e}")
            return False
    
    def test_gateway_status(self):
        """Test the gateway status endpoint"""
        try:
            status_url = f"{self.base_url}/api/gateway/status"
            print(f"Testing gateway status at {status_url}")
            
            response = requests.get(status_url, timeout=5)
            print(f"Status check status: {response.status_code}")
            
            if response.status_code == 200:
                status_data = response.json()
                print(f"Status check response: {json.dumps(status_data, indent=2)}")
                return True
            else:
                print(f"Status check failed with status: {response.status_code}")
                return False
                
        except Exception as e:
            print(f"Status check failed: {e}")
            return False
    
    def test_gateway_metrics(self):
        """Test the gateway metrics endpoint"""
        try:
            metrics_url = f"{self.base_url}/api/gateway/metrics"
            print(f"Testing gateway metrics at {metrics_url}")
            
            response = requests.get(metrics_url, timeout=5)
            print(f"Metrics check status: {response.status_code}")
            
            if response.status_code == 200:
                metrics_data = response.json()
                print(f"Metrics check response: {json.dumps(metrics_data, indent=2)}")
                return True
            else:
                print(f"Metrics check failed with status: {response.status_code}")
                return False
                
        except Exception as e:
            print(f"Metrics check failed: {e}")
            return False
    
    def test_404_fallback(self):
        """Test the 404 fallback for unmatched routes"""
        try:
            invalid_url = f"{self.base_url}/invalid/route"
            print(f"Testing 404 fallback at {invalid_url}")
            
            response = requests.get(invalid_url, timeout=5)
            print(f"404 fallback status: {response.status_code}")
            
            if response.status_code == 404:
                error_data = response.json()
                print(f"404 fallback response: {json.dumps(error_data, indent=2)}")
                return True
            else:
                print(f"404 fallback test failed - expected 404, got {response.status_code}")
                return False
                
        except Exception as e:
            print(f"404 fallback test failed: {e}")
            return False
    
    def test_gateway_connectivity(self):
        """Test basic connectivity to gateway"""
        try:
            print(f"Testing basic connectivity to {self.base_url}")
            
            # Try to connect to the gateway
            response = requests.get(self.base_url, timeout=5)
            print(f"Connectivity test status: {response.status_code}")
            
            # Any response means the gateway is reachable
            return True
                
        except requests.exceptions.ConnectionError:
            print("Connection failed - gateway may not be running")
            return False
        except Exception as e:
            print(f"Connectivity test failed: {e}")
            return False

def main():
    """Main test function"""
    print("=== Flex.Gateway.Yarp Basic Test Suite ===")
    
    # Create tester instance
    tester = GatewayTester()
    
    # Test 1: Basic connectivity
    print("\n1. Testing Gateway Connectivity")
    connectivity_ok = tester.test_gateway_connectivity()
    
    # Test 2: Health check
    print("\n2. Testing Gateway Health Check")
    health_ok = tester.test_health_check()
    
    # Test 3: Gateway status
    print("\n3. Testing Gateway Status")
    status_ok = tester.test_gateway_status()
    
    # Test 4: Gateway metrics
    print("\n4. Testing Gateway Metrics")
    metrics_ok = tester.test_gateway_metrics()
    
    # Test 5: 404 fallback
    print("\n5. Testing 404 Fallback")
    fallback_ok = tester.test_404_fallback()
    
    # Summary
    print("\n=== Test Summary ===")
    print(f"Connectivity: {'✓' if connectivity_ok else '✗'}")
    print(f"Health Check: {'✓' if health_ok else '✗'}")
    print(f"Status Check: {'✓' if status_ok else '✗'}")
    print(f"Metrics Check: {'✓' if metrics_ok else '✗'}")
    print(f"404 Fallback: {'✓' if fallback_ok else '✗'}")
    
    passed_tests = sum([connectivity_ok, health_ok, status_ok, metrics_ok, fallback_ok])
    total_tests = 5
    
    print(f"\nPassed: {passed_tests}/{total_tests} tests")
    
    if passed_tests == total_tests:
        print("\n🎉 All tests passed! Gateway is working correctly.")
    elif passed_tests > 0:
        print(f"\n⚠️  {passed_tests}/{total_tests} tests passed. Some functionality may not be working.")
    else:
        print("\n❌ No tests passed. Please check if the gateway is running and accessible.")
        print("\nTo start the gateway:")
        print("1. Navigate to the gateway directory")
        print("2. Run: dotnet run")
        print("3. Make sure the gateway is listening on port 5000")

if __name__ == "__main__":
    main()
