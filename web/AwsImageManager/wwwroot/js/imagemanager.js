const STATUS_INITIAL = 0, STATUS_SAVING = 1, STATUS_SUCCESS = 2, STATUS_FAILED = 3;

Vue.component('image-list', {
    template: `<div>
         
            <section class="jumbotron text-center">
                <div class="container">
                    <h1 class="jumbotron-heading" v-if="loading">Loading Images...</h1>
                    <h1 class="jumbotron-heading" v-if="!loading">You have {{ imageList.length }} images.</h1>
                    <p v-if="!loading" class="lead text-muted">Wanna add somemore?</p>
                    <image-uploader @uploadSuccess="updatePhotoList"></image-uploader>
                </div>
            </section>

            <div class="album text-muted">
                <div class="container">
                    <div class="row">
                        <image-thumbnail v-for="image in imageList" @imageDeleted="updatePhotoList" v-bind:model="image"></image-thumbnail>
                    </div>
                </div>
            </div>
        </div>`,
    data: () => ({
        imageList: [],
        loading: true
    }),
    created: function () {
        this.updatePhotoList();
    },
    methods: {
        updatePhotoList() {
            this.loading = true;
            axios.get(`/api/images`)
                .then(response => {
                    this.loading = false;
                    this.imageList = response.data;
                    console.log(this.imageList);
                })
                .catch(e => {
                    this.loading = false;
                    this.errors.push(e)
                });
        }
    }
});

Vue.component('image-thumbnail', {
    props: ['model'],
    template: `
            <div class="card">
                <img :src="model.url" alt="100%x280" style="height: 280px; width: 100%; display: block;" data-holder-rendered="true">
                <button type="button" class="btn btn-outline-danger btn-sm delete" v-on:click="deleteImage">X</button>
                <p class="card-text">
                    <button class="btn btn-sm tag" style="margin:2px;" v-for="tag in model.tags">
                      {{ tag.name }} <span class="badge badge-secondary">{{tag.confidence}}</span>
                    </button>
                </p>
            </div>`,
    methods: {
        deleteImage() {
            axios.delete(`/api/images/` + this.model.key)
                .then(response => {
                    this.$emit('imageDeleted');
                });
        }
    }
});

Vue.component('image-uploader', {
    template: `<form enctype="multipart/form-data" novalidate>
            <div class="dropbox">
              <input type="file" :name="uploadFieldName" :disabled="isSaving" @change="filesChange($event.target.name, $event.target.files); fileCount = $event.target.files.length" accept="image/*" class="input-file">
                <p v-if="!isSaving">
                  Drag your file here to begin<br> or click to browse
                </p>
                <p v-if="isSaving">
                  Uploading {{ fileCount }} files...
                </p>
            </div>
        </form>`,
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
            return axios.post(url, formData)
                // get data
                .then(x => x.data)
                // add url field
                .then(x => console.log(x));
        }
    },
    mounted() {
        this.reset();
    }
});

var app = new Vue({
    el: '#app'
});