pipeline {
  agent any

  parameters {
        dynamicParameter(
            name: 'SOURCE_BRANCH',
            script: '''
                def branches = "git ls-remote --heads https://gitlab.fss.com.vn/sec/customers/tcb/cbplus | awk -F'/' '{print $3}'".execute().text.readLines()
                return branches
            ''',
            description: 'Select the branch to build'
        )

        choice(
            name: 'USE_CACHE',
            choices: ['true', 'false'],
            description: 'Use Docker cache?'
        )
    }

  environment {
    GIT_SERVICE_CREDS = 'devops_gitlab'
    REGISTRY = 'docker-registry.fss.com.vn'
    REPO_URL = "https://gitlab.fss.com.vn/sec/customers/tcb/cbplus"
    IMAGE_NAME = "tcb/hostservice"
    BRANCH_NAME = "${params.SOURCE_BRANCH}"
    BUILD_IMG_DIR = "."
    DOCKERFILE_PATH = "Dockerfile"
    CONTEXT_PATH = '.'
    dockerImage = ''
    lastCommit = ''
  }

  stages {
    stage('Check out') {
      steps {
        echo 'Checking out source code from: ' + REPO_URL + ', branch: ' + BRANCH_NAME
        checkout([$class: 'GitSCM', 
                  branches: [[name: BRANCH_NAME]],
                  userRemoteConfigs: [[credentialsId: GIT_SERVICE_CREDS, url: REPO_URL]]])

        script {
          lastCommit = sh(
          returnStdout: true,
          script: 'git rev-parse --verify HEAD | tr "\n" " "'
        )
          echo '===============Last commit: ' + lastCommit
        }
        sh 'echo .git >> .dockerignore'
      }
    }

    stage('Build Docker Image') {
      steps {
        echo 'Starting to build docker image: ' + DOCKERFILE_PATH
        dir("${BUILD_IMG_DIR}") {
          script {
                def buildArgs = params.USE_CACHE == 'false' ? '--no-cache' : ''
                dockerImage = docker.build("${REGISTRY}/${IMAGE_NAME}", 
                    buildArgs + ' --build-arg LAST_COMMIT=' + lastCommit + 
                    ' --build-arg BRANCH_NAME=' + BRANCH_NAME + 
                    ' -f ' + DOCKERFILE_PATH + ' ' + CONTEXT_PATH)
            }
        }
      }
      post {
        success {
          echo '========Build image ' + IMAGE_NAME + ' successfully========'
        }
        failure {
          echo '========Build image ' + IMAGE_NAME + ' failed=============='
        }
      }
    }

    stage('Push Docker Image') {
      steps {
        echo 'Starting to push docker image to: https://' + REGISTRY
        script {
          withCredentials([usernamePassword(credentialsId: 'robot$devops', 
                                            usernameVariable: 'USERNAME', 
                                            passwordVariable: 'PASSWORD')]) {
            docker.withRegistry('https://' + REGISTRY, 'robot$devops') {
              dockerImage.push('latest')
            }
          }
        }
      }
      post {
        success {
          echo '========Push image ' + IMAGE_NAME + ' successfully========'
        }
        failure {
          echo '========Push image ' + IMAGE_NAME + ' failed=============='
        }
      }
    }
  }
}
