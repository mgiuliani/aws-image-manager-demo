<template>
  <div>
    <form enctype="multipart/form-data" novalidate="">
      <div class="dropbox">
        <input type="file" @change="filesChange($event.target.name, $event.target.files); fileCount = $event.target.files.length" accept="image/*" class="input-file" />
        <p v-if="!isSaving">Drag your file here to begin<br /> or click to browse</p>
        <p v-if="isSaving">Uploading {{ fileCount }} files...</p>
      </div>
    </form>
  </div>
</template>

<script>
import Axios from "axios";

const STATUS_INITIAL = 0, STATUS_SAVING = 1, STATUS_SUCCESS = 2, STATUS_FAILED = 3;

export default {
  name: 'image-uploader',
  data() {
        return {
            uploadedFiles: [],
            uploadError: null,
            currentStatus: null,
            uploadFieldName: 'file'
        }
    },
    computed: {
        isInitial() {
            return this.currentStatus === STATUS_INITIAL;
        },
        isSaving() {
            return this.currentStatus === STATUS_SAVING;
        },
        isSuccess() {
            return this.currentStatus === STATUS_SUCCESS;
        },
        isFailed() {
            return this.currentStatus === STATUS_FAILED;
        }
    },
    methods: {
        reset() {
            // reset form to initial state
            this.currentStatus = STATUS_INITIAL;
            this.uploadedFiles = [];
            this.uploadError = null;
        },
        save(formData) {
            // upload data to the server
            this.currentStatus = STATUS_SAVING;

            this.upload(formData)
                .then(x => {
                    this.uploadedFiles = [].concat(x);
                    this.currentStatus = STATUS_SUCCESS;

                    this.$emit('uploadSuccess');
                })
                .catch(err => {
                    this.uploadError = err.response;
                    this.currentStatus = STATUS_FAILED;
                });
        },
        filesChange(fieldName, fileList) {
            // handle file changes
            const formData = new FormData();

            if (!fileList.length) return;

            // append the files to FormData
            Array
                .from(Array(fileList.length).keys())
                .map(x => {
                    formData.append(fieldName, fileList[x], fileList[x].name);
                });

            // save it
            this.save(formData);
        },
        upload(formData) {
            const url = '/api/images';
            return Axios.post(url, formData)
                // get data
                .then(x => x.data)
                // add url field
                .then(x => console.log(x));
        }
    },
    mounted() {
        this.reset();
    }
}
</script>