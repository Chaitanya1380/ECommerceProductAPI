pipeline {
    agent any

    environment {
        DOCKER_IMAGE = "yourdockerhubusername/yourappname"
        DOCKER_TAG = "latest"
        REGISTRY_CREDENTIALS = "docker-hub-credentials-id"
    }

    stages {

        stage('Build Docker Image') {
            steps {
                script {
                    docker.build("${DOCKER_IMAGE}:${DOCKER_TAG}")
                }
            }
        }

        stage('Push to Registry') {
            steps {
                script {
                    docker.withRegistry('', "${REGISTRY_CREDENTIALS}") {
                        docker.image("${DOCKER_IMAGE}:${DOCKER_TAG}").push()
                    }
                }
            }
        }

        stage('Cleanup') {
            steps {
                sh 'docker rmi ${DOCKER_IMAGE}:${DOCKER_TAG} || true'
            }
        }
    }

    post {
        success {
            echo 'Docker image built and pushed successfully!'
        }
        failure {
            echo 'Build failed. Check logs.'
        }
    }
}
