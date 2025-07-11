#!/usr/bin/env python3
"""
Simple script to test MediaService file upload endpoints
"""
import requests
import json
from pathlib import Path

# Create a simple test image file (1x1 pixel PNG)
test_image_data = b'\x89PNG\r\n\x1a\n\x00\x00\x00\rIHDR\x00\x00\x00\x01\x00\x00\x00\x01\x08\x02\x00\x00\x00\x90wS\xde\x00\x00\x00\tpHYs\x00\x00\x0b\x13\x00\x00\x0b\x13\x01\x00\x9a\x9c\x18\x00\x00\x00\x0bIDAT\x08\x99c\xf8\x0f\x00\x00\x01\x01\x01\x00\x18\xdd\x8d\xb4\x00\x00\x00\x00IEND\xaeB`\x82'

# Save test image
with open('test-image.png', 'wb') as f:
    f.write(test_image_data)

print("Created test-image.png")

# Test single image upload
base_url = "http://localhost:5154"

def test_single_upload():
    print("\n=== Testing Single Image Upload ===")
    
    url = f"{base_url}/api/media/upload"
    
    with open('test-image.png', 'rb') as f:
        files = {
            'file': ('test-image.png', f, 'image/png')
        }
        data = {
            'productId': 'test-product-123',
            'alt': 'Test image for upload',
            'isPrimary': 'true'
        }
        
        try:
            response = requests.post(url, files=files, data=data)
            print(f"Status Code: {response.status_code}")
            print(f"Response: {response.text}")
            
            if response.status_code == 200:
                result = response.json()
                print(f"✅ Upload successful!")
                print(f"File URL: {result.get('fileUrl')}")
                print(f"Thumbnail URL: {result.get('thumbnailUrl')}")
                return result
            else:
                print(f"❌ Upload failed")
                return None
                
        except Exception as e:
            print(f"❌ Error: {e}")
            return None

def test_batch_upload():
    print("\n=== Testing Batch Image Upload ===")
    
    url = f"{base_url}/api/media/batch-upload"
    
    # Create multiple test files
    files = []
    for i in range(3):
        filename = f'test-image-{i}.png'
        with open(filename, 'wb') as f:
            f.write(test_image_data)
        files.append(('files', (filename, open(filename, 'rb'), 'image/png')))
    
    data = {
        'productId': 'test-product-batch-123'
    }
    
    try:
        response = requests.post(url, files=files, data=data)
        print(f"Status Code: {response.status_code}")
        print(f"Response: {response.text}")
        
        # Close file handles
        for _, (_, file_handle, _) in files:
            file_handle.close()
        
        if response.status_code == 200:
            result = response.json()
            print(f"✅ Batch upload successful!")
            print(f"Uploaded {len(result)} files")
            for i, item in enumerate(result):
                print(f"  File {i+1}: {item.get('fileUrl')}")
            return result
        else:
            print(f"❌ Batch upload failed")
            return None
            
    except Exception as e:
        print(f"❌ Error: {e}")
        return None

def test_health_check():
    print("\n=== Testing Health Check ===")
    
    url = f"{base_url}/health"
    
    try:
        response = requests.get(url)
        print(f"Status Code: {response.status_code}")
        print(f"Response: {response.text}")
        
        if response.status_code == 200:
            print("✅ Health check passed")
            return True
        else:
            print("❌ Health check failed")
            return False
            
    except Exception as e:
        print(f"❌ Error: {e}")
        return False

if __name__ == "__main__":
    print("MediaService Upload Test Script")
    print("=" * 40)
    
    # Test health check first
    if not test_health_check():
        print("Service is not healthy, stopping tests")
        exit(1)
    
    # Test single upload
    single_result = test_single_upload()
    
    # Test batch upload
    batch_result = test_batch_upload()
    
    print("\n" + "=" * 40)
    print("Test Results Summary:")
    print(f"Health Check: {'✅ PASSED' if True else '❌ FAILED'}")
    print(f"Single Upload: {'✅ PASSED' if single_result else '❌ FAILED'}")
    print(f"Batch Upload: {'✅ PASSED' if batch_result else '❌ FAILED'}")
    
    # Cleanup
    import os
    for file in ['test-image.png', 'test-image-0.png', 'test-image-1.png', 'test-image-2.png']:
        if os.path.exists(file):
            os.remove(file)
    
    print("\nTest files cleaned up.")
